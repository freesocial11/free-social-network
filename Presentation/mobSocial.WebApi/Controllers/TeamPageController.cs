using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using mobSocial.Data.Entity.GroupPages;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.TeamPages;
using mobSocial.Data.Helpers;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.TeamPages;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Extensions.ModelExtensions;
using mobSocial.WebApi.Models.TeamPages;
using mobSocial.WebApi.Models.Users;

namespace mobSocial.WebApi.Controllers
{
    [RoutePrefix("teampage")]
    public class TeamPageApiController : RootApiController
    {
        private readonly ITeamPageService _teamPageService;
        private readonly ITeamPageGroupService _teamPageGroupService;
        private readonly ITeamPageGroupMemberService _teamPageGroupMemberService;
        private readonly IUserService _userService;
        private readonly IMediaService _mediaService;
        private readonly MediaSettings _mediaSettings;

        public TeamPageApiController(ITeamPageService teamPageService, 
            ITeamPageGroupService teamPageGroupService, 
            ITeamPageGroupMemberService teamPageGroupMemberService, 
            IUserService userService, 
            IMediaService mediaService, 
            MediaSettings mediaSettings)
        {
            _teamPageService = teamPageService;
            _teamPageGroupService = teamPageGroupService;
            _teamPageGroupMemberService = teamPageGroupMemberService;
            _userService = userService;
            _mediaService = mediaService;
            _mediaSettings = mediaSettings;
        }

        [HttpPost]
        [Authorize]
        [Route("post")]
        public IHttpActionResult Post(TeamPageModel model)
        {
            if(!ModelState.IsValid || model == null)
                return Response(new { Success = false, Message = "Invalid data" });

            var teamPage = new TeamPage
            {
                Description = model.Description,
                Name = model.Name,
                TeamPictureId=  model.TeamPictureId,
                CreatedBy = ApplicationContext.Current.CurrentUser.Id,
                CreatedOn = DateTime.UtcNow,
                UpdatedOn = DateTime.UtcNow
            };
            //save to db now
            _teamPageService.Insert(teamPage);

            return RespondSuccess(new
            {
               TeamPage = teamPage.ToModel(_mediaService)
            });
        }

        [HttpPut]
        [Authorize]
        [Route("put")]
        public IHttpActionResult Put(TeamPageModel model)
        {
            if (!ModelState.IsValid || model == null || model.Id == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            var teamPage = _teamPageService.Get(model.Id);
            //check if the page exists or not & the person editing actually owns the resource
            if (teamPage.CreatedBy != ApplicationContext.Current.CurrentUser.Id && !ApplicationContext.Current.CurrentUser.IsAdministrator())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            teamPage.Description = model.Description;
            teamPage.Name = model.Name;
            teamPage.TeamPictureId = model.TeamPictureId;
          
            //update the updation date
            teamPage.UpdatedOn = DateTime.UtcNow;
            //update now
            _teamPageService.Update(teamPage);

            return RespondSuccess(new {
                TeamPage = teamPage.ToModel(_mediaService)
            });
        }

        [HttpPut]
        [Authorize]
        [Route("cover/put/{id:int}/{pictureId:int}")]
        public IHttpActionResult UpdateTeamPicture(int id, int pictureId)
        {
            //first retrieve the team page
            var teamPage = _teamPageService.Get(id);
            //is the current user authorized to perform this operation
            if (teamPage == null ||
                (teamPage.CreatedBy != ApplicationContext.Current.CurrentUser.Id && !ApplicationContext.Current.CurrentUser.IsAdministrator()))
            {
                return Json(new
                {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            
            teamPage.TeamPictureId = pictureId;

            //save now
            _teamPageService.Update(teamPage);

            return Json(new
            {
                Success = true
            });

        }

        [HttpGet]
        [Route("get/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var teamPage = _teamPageService.Get(id);
            if (teamPage == null)
            {
                return NotFound();
            }
            var model = teamPage.ToEntityModel(_mediaService);

            model.Groups = GetTeamPageGroupPublicModels(id);
            var currentUser = ApplicationContext.Current.CurrentUser;
            //is the page editable
            model.IsEditable = currentUser != null && (currentUser.IsAdministrator() || currentUser.Id == teamPage.CreatedBy);

            model.TeamPictureUrl = _mediaService.GetPictureUrl(model.TeamPictureId);
            return Response(new
            {
                Success = true,
                TeamPage = model
            });
        }

        [HttpGet]
        [Route("get/my")]
        [Authorize]
        public IHttpActionResult Get()
        {
            var teamPages = _teamPageService.GetTeamPagesByOwner(ApplicationContext.Current.CurrentUser.Id);

            var model = new List<TeamPageModel>();
            foreach (var page in teamPages)
            {
                var pModel = page.ToEntityModel(_mediaService);
                model.Add(pModel);
            }

            return Response(new {
                Success = true,
                TeamPages = model
            });
        }

        [HttpGet]
        [Route("get/{seName}")]
        public IHttpActionResult Get(string seName)
        {
            var teamPage = _teamPageService.GetBySeName(seName);
            if (teamPage == null)
                return NotFound();

            var model = teamPage.ToModel(_mediaService);

            return Response(new {
                Success = true,
                TeamPage = model
            });
        }

        [HttpDelete]
        [Route("delete/{id:int}")]
        [Authorize]
        public IHttpActionResult Delete(int id)
        {
            var teamPage = _teamPageService.Get(id);
            if (teamPage == null)
            {
                return NotFound();
            }
            var currentUser = ApplicationContext.Current.CurrentUser;
            //check if the page exists or not & the person deleting actually owns the resource
            if (teamPage.CreatedBy != currentUser.Id && !currentUser.IsAdministrator())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            //delete the team page safely
            _teamPageService.SafeDelete(teamPage);

            return Response(new {
                Success = true
            });
        }

        [HttpPost]
        [Authorize]
        [Route("group/post")]
        public IHttpActionResult PostGroup(TeamPageGroupModel model)
        {
            if(!ModelState.IsValid || model == null || model.TeamPageId == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            //check if the team page exists? and if it does, is the person creating the group has the authority
            var teamPage = _teamPageService.Get(model.TeamPageId);
            if (teamPage == null)
            {
                return NotFound();
            }
            var currentUser = ApplicationContext.Current.CurrentUser;
            //check if the page exists or not & the person deleting actually owns the resource
            if (currentUser != null && teamPage.CreatedBy != currentUser.Id && ! currentUser.IsAdministrator())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            //ok, so we are good to save the group
            var group = new GroupPage()
            {
                TeamPageId = model.TeamPageId,
                Name = model.Name,
                Description = model.Description,
                PayPalDonateUrl = model.PayPalDonateUrl,
                DisplayOrder = model.DisplayOrder,
                IsDefault = model.IsDefault
            };

            _teamPageGroupService.Insert(group);
            return Response(new {
                Success = true,
                Id = group.Id
            });
        }
        [HttpPut]
        [Authorize]
        [Route("group/put")]
        public IHttpActionResult PutGroup(TeamPageGroupModel model)
        {
            if (!ModelState.IsValid || model == null || model.TeamPageId == 0 || model.Id == 0)
                return Response(new { Success = false, Message = "Invalid data" });

            //check if the team page exists? and if it does, is the person creating the group has the authority
            var teamPage = _teamPageService.Get(model.TeamPageId);
            if (teamPage == null)
            {
                return NotFound();
            }
            var currentUser = ApplicationContext.Current.CurrentUser;
            //check if the page exists or not & the person deleting actually owns the resource
            if (teamPage.CreatedBy != currentUser.Id && !currentUser.IsAdministrator())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
           //retrieve the group
            var groupPage = _teamPageGroupService.Get(model.Id);
            if (groupPage == null)
            {
                return NotFound();
            }

            groupPage.Name = model.Name;
            groupPage.Description = model.Description;
            groupPage.DisplayOrder = model.DisplayOrder;
            groupPage.PayPalDonateUrl = model.PayPalDonateUrl;
            groupPage.TeamPageId = model.TeamPageId;
            groupPage.IsDefault = model.IsDefault;
            groupPage.DisplayOrder = model.DisplayOrder;
            groupPage.DateUpdated = DateTime.UtcNow;
            

            //if the current group is default group, the other one should be set as non-default
            if (model.IsDefault)
            {
                //first get all groups of this team
                var groupPages = _teamPageGroupService.GetGroupPagesByTeamId(model.TeamPageId);
                foreach (var gp in groupPages)
                {
                    if (gp.Id != groupPage.Id)
                    {
                        //set default false and update the group
                        gp.IsDefault = false;
                        _teamPageGroupService.Update(gp);
                    }
                }
            }

            //update the current group page now
            _teamPageGroupService.Update(groupPage);
            return Response(new {
                Success = true,
                Id = groupPage.Id
            });
        }

        [Route("group/delete/{groupId:int}")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteGroup(int groupId)
        {
            //first retrieve the group and make sure that the right person is deleting the group
            var team = _teamPageService.GetTeamPageByGroup(groupId);
            if (team == null || team.CreatedBy != ApplicationContext.Current.CurrentUser.Id && !ApplicationContext.Current.CurrentUser.IsAdministrator())
            {
                return Response(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            //get the group
            var group = team.GroupPages.FirstOrDefault(x => x.Id == groupId);
            if (group == null)
            {
                return NotFound();
            }
            //if there are more than one group and it's the default group that's being deleted
            if (group.IsDefault)
            {
                if (team.GroupPages.Count > 1)
                {
                    return Response(new
                    {
                        Success = false,
                        Message = "Can't delete the default group"
                    });
                }
                else
                {
                    //this is the last group, so safe delete
                    _teamPageGroupService.SafeDelete(group);
                }
            }
            //safe delete the group. replacement for the code written below
            _teamPageGroupService.SafeDelete(group);

            //the code below was commented to avoid any confusion regarding deletion
            //todo: decide whether we should keep the code below
            /*
            else
            {
                //since it's not a default group, we'll need to move all members to default group
                //find the default group for this page
                var defaultGroup = team.GroupPages.FirstOrDefault(x => x.IsDefault);
                if (defaultGroup == null)
                {
                    //this should not hit unless direct changes have been made to database. we safe delete the group in this case
                    _teamPageGroupService.SafeDelete(group);
                }
                else
                {
                    //move all the members of this group to default group
                    foreach (var member in group.Members)
                    {
                        member.GroupPageId = defaultGroup.Id;
                        _teamPageGroupMemberService.Update(member);
                    }

                    //delete the group now
                    _teamPageGroupService.Delete(group);
                }
            }*/
            return Response(new {Success = true});

        }

        [Route("group/get/{teamId:int}")]
        [HttpGet]
        public IHttpActionResult GetTeamGroups(int teamId)
        {
            //first check if the team exists?
            var teamPage = _teamPageService.Get(teamId);
            if (teamPage == null)
            {
                return NotFound();
            }

            var listTeamPageGroups = GetTeamPageGroupPublicModels(teamId);

            //send the response
            return Json(new
            {
                Success = true,
                TeamGroups = listTeamPageGroups
            });
        }

        [Authorize]
        [HttpPost]
        [Route("group/members/post")]
        public IHttpActionResult PostGroupMembers(TeamPageGroupMemberModel model)
        {
            //we'll have to check each group and then each member in question to see if they both exist and that a 
            //group member entry already exist for the combination
            //lets first query all the members
            var members = _userService.Get(x => model.CustomerId.Contains(x.Id)).ToList();
            if(members.Count == 0)
                return Json(new
                {
                    Success = false,
                    Message = "Members don't exist"
                });

            //does this team exist
            var team = _teamPageService.Get(model.TeamId);
            if (team == null ||
                (team.CreatedBy != ApplicationContext.Current.CurrentUser.Id && !ApplicationContext.Current.CurrentUser.IsAdministrator()))
            {
                return Json(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }

            
            //if user hasn't passed any group id, it's better to add default group for processing
            if (model.GroupId.Length == 0)
            {
                var defaultGroup = team.GroupPages.FirstOrDefault(x => x.IsDefault);
                if (defaultGroup == null)
                {
                    defaultGroup = team.GroupPages.FirstOrDefault();
                    if (defaultGroup == null)
                    {
                        return Json(new {
                            Success = false,
                            Message = "No groups found in the team page"
                        });
                    }
                }
                model.GroupId = new[] {defaultGroup.Id};
            }
            
            foreach (var groupId in model.GroupId)
            {
                //check if the group exist
                var group = team.GroupPages.FirstOrDefault(x => x.Id == groupId);
                if (group == null)
                {
                    continue; //skip as group doesn't exist
                }

                //let's find existing group members
                var groupMembers = group.Members;

                //if group validation succeed, we loop through the members and add those members which don't exist
                foreach (var member in members)
                {
                    if (groupMembers.All(x => x.CustomerId != member.Id))
                    {
                        //let's add this combination
                        _teamPageGroupMemberService.Insert(new GroupPageMember()
                        {
                            CustomerId = member.Id,
                            GroupPageId = group.Id,
                            DisplayOrder = 0
                        });
                    }
                }
            }
            return Json(new
            {
                Success = true
            });
        }

        [Route("group/members/delete/{groupId}/{memberId}")]
        [HttpDelete]
        [Authorize]
        public IHttpActionResult DeleteGroupMember(int groupId, int memberId)
        {
            //first check if the group exist?
            var group = _teamPageGroupService.Get(groupId);
            if (group == null)
            {
                return NotFound();
            }

            //check if the user adding is authorized to do that
            if (group.Team.CreatedBy != ApplicationContext.Current.CurrentUser.Id && !ApplicationContext.Current.CurrentUser.IsAdministrator())
            {
                return Json(new {
                    Success = false,
                    Message = "Unauthorized"
                });
            }
            //delete the page member
            _teamPageGroupMemberService.DeleteGroupPageMember(groupId, memberId);

            return Json(new
            {
                Success = true
            });

        }

      
        #region helpers

        private List<TeamPageGroupPublicModel> GetTeamPageGroupPublicModels(int teamId)
        {
            //retrieve all group pages by team
            var groupPages = _teamPageGroupService.GetGroupPagesByTeamId(teamId);

            var listTeamPageGroups = new List<TeamPageGroupPublicModel>();

            //get all group members for performance
            var allMembers = _teamPageGroupMemberService.GetGroupPageMembersForTeam(teamId);
            var allMembersCustomerIds = allMembers.Select(x => x.CustomerId);

            var allCustomers = _userService.Get(x => allMembersCustomerIds.Contains(x.Id)).ToList();
            foreach (var groupPage in groupPages)
            {
                var groupMembers = allMembers.Where(x => x.GroupPageId == groupPage.Id).OrderBy(x => x.DisplayOrder);
                //setup the individual group model
                var groupModel = groupPage.ToModel();
                //add customers' public models to the list
                foreach (var member in groupMembers)
                {
                    var memberCustomer = allCustomers.First(x => x.Id == member.CustomerId);
                    groupModel.GroupMembers.Add(memberCustomer.ToModel(_mediaService, _mediaSettings));
                }

                //add this group model to the output list
                listTeamPageGroups.Add(groupModel);
            }
            return listTeamPageGroups;
        }
        #endregion

    }
}