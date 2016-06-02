using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using mobSocial.Data.Entity.ArtistPages;
using mobSocial.Data.Entity.MediaEntities;
using mobSocial.Data.Entity.Settings;
using mobSocial.Data.Entity.Songs;
using mobSocial.Data.Helpers;
using mobSocial.Services.ArtistPages;
using mobSocial.Services.Extensions;
using mobSocial.Services.Helpers;
using mobSocial.Services.MediaServices;
using mobSocial.Services.Music;
using mobSocial.Services.Social;
using mobSocial.Services.Songs;
using mobSocial.Services.Users;
using mobSocial.WebApi.Configuration.Infrastructure;
using mobSocial.WebApi.Configuration.Mvc;
using mobSocial.WebApi.Models.ArtistPages;
using mobSocial.WebApi.Models.Pictures;
using Microsoft.Owin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace mobSocial.WebApi.Controllers
{

    [RoutePrefix("api/artists")]
    public partial class ArtistPageController : RootApiController
    {
        #region variables

        private readonly IMediaService _pictureService;
        private readonly IUserService _userService;
        private readonly UserSettings _customerSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IArtistPageService _artistPageService;
        private readonly IArtistPageApiService _artistPageApiService;
        private readonly IArtistPageManagerService _artistPageManagerService;
        private readonly ISongService _songService;
        private readonly IArtistPagePaymentService _artistPagePaymentService;
        private readonly IMusicService _musicService;
        private readonly IFriendService _friendService;

        public ArtistPageController(IMediaService pictureService,
            IUserService userService,
            UserSettings customerSettings,
            MediaSettings mediaSettings,
            IArtistPageService artistPageService,
            IArtistPageApiService artistPageApiService,
            IArtistPageManagerService artistPageManagerService,
            ISongService songService,
            IArtistPagePaymentService artistPagePaymentService, 
            IMusicService musicService, 
            IFriendService friendService)
        {
            _pictureService = pictureService;
            _userService = userService;
            _customerSettings = customerSettings;
            _mediaSettings = mediaSettings;
            _artistPageService = artistPageService;
            _artistPageApiService = artistPageApiService;
            _artistPageManagerService = artistPageManagerService;
            _songService = songService;
            _artistPagePaymentService = artistPagePaymentService;
            _musicService = musicService;
            _friendService = friendService;
        }

        #endregion

        #region Actions
        [Route("get/{id:int}")]
        public IHttpActionResult Get(int id)
        {
            var artist = _artistPageService.Get(id);

            if (artist == null)
                return NotFound();

            var model = new ArtistPageModel() {
                Description = artist.Biography,
                DateOfBirth = artist.DateOfBirth,
                Gender = artist.Gender,
                Name = artist.Name,
                ShortDescription = artist.ShortDescription,
                HomeTown = artist.HomeTown,
                RemoteEntityId = artist.RemoteEntityId,
                RemoteSourceName = artist.RemoteSourceName,
                Id = artist.Id
            };

            //images for artist
            foreach (var picture in artist.GetPictures())
            {
                model.Pictures.Add(new PictureResponseModel {
                    Id = picture.Id,
                    PictureUrl = _pictureService.GetPictureUrl(picture),
                });
            }

            model.CanEdit = CanEdit(artist);
            model.CanDelete = CanDelete(artist);

            return Response(model);
        }
        /// <summary>
        /// Imports a new artist from remote api if it doesn't exist in our database
        /// </summary>
        public IHttpActionResult RemoteArtist(string remoteEntityId)
        {
            if (!string.IsNullOrEmpty(remoteEntityId))
            {
                var artists = _artistPageService.GetArtistPagesByRemoteEntityId(new string[] { remoteEntityId });
                if (artists.Count == 0)
                {
                    //we need to create a new artist now
                    var remoteArtist = _artistPageApiService.GetRemoteArtist(remoteEntityId);
                    if (remoteArtist == null)
                        return NotFound();

                    var artist = SaveRemoteArtistToDB(remoteArtist);
                    return RedirectToRoute("ArtistPageUrl", new { SeName = artist.GetPermalink() });
                }
                else
                {
                    //the page already exists in our database. No need to create duplicate entries. Rather redirect them to the actual artist page
                    return RedirectToRoute("ArtistPageUrl", new { SeName = artists[0].GetPermalink() });
                }
            }
            //totally unknown path
            return NotFound();
        }

        [HttpGet]
        [Route("getrelatedartists/{artistId:int}")]
        public IHttpActionResult GetRelatedArtists(int artistId)
        {
            var artistPage = _artistPageService.Get(artistId);
            var model = new List<object>();
            if (string.IsNullOrEmpty(artistPage?.RemoteEntityId)) //if it's not a remote artist means some user has created it. so no related artists
                return Response(model);

            var relatedArtistsRemoteCollection = _artistPageApiService.GetRelatedArtists(artistPage.RemoteEntityId);
            if (relatedArtistsRemoteCollection == null)
                return Response(model);


            //get all the remote entity ids from the remote collection. We'll see those ids in our database to find matches
            //we'll deserialize the list to get the object items
            var relatedArtistDeserialized = new List<JObject>();
            foreach (var rarcItem in relatedArtistsRemoteCollection)
            {
                relatedArtistDeserialized.Add((JObject)JsonConvert.DeserializeObject(rarcItem));
            }
            //get the entity ids
            var relatedArtistsRemoteEntityIds = relatedArtistDeserialized.Select(x => x["RemoteEntityId"].ToString());

            //get all the related artists which we already have in our database
            var relatedArtistsInDB = _artistPageService.GetArtistPagesByRemoteEntityId(relatedArtistsRemoteEntityIds.ToArray());

            var relatedArtistsInDBIds = relatedArtistsInDB.Select(m => m.RemoteEntityId).ToList();

            //lets now find the ones which we don't have in our database. we'll save them by importing
            var relatedArtistsToBeImportedIds = relatedArtistsRemoteEntityIds.Except(relatedArtistsInDBIds).ToList();

            foreach (var reid in relatedArtistsToBeImportedIds)
            {
                var artistJson = relatedArtistDeserialized.First(x => x["RemoteEntityId"].ToString() == reid).ToString();
                ArtistPage relatedartistPage = SaveRemoteArtistToDB(artistJson);
                relatedArtistsInDB.Add(relatedartistPage); //add new page to list of db pages

            }

            foreach (var ra in relatedArtistsInDB)
            {
                var imageUrl = "";
                var pictures = ra.GetPictures();
                if (pictures != null && pictures.Count > 0)
                    imageUrl = _pictureService.GetPictureUrl(pictures.First());

                model.Add(new {
                    Name = ra.Name,
                    Id = ra.Id,
                    ImageUrl = imageUrl,
                    ShortDescription = ra.ShortDescription,
                    SeName = ra.GetPermalink(),
                    RemoteArtist = false
                });
            }

            return Response(model);
        }

        [HttpGet]
        [Route("getartistsongs/{artistName}")]
        public IHttpActionResult GetArtistSongs(string artistName)
        {
            if (string.IsNullOrEmpty(artistName))
                return null;

            var model = new List<object>();
            //get songs from remote server
            var songs = _artistPageApiService.GetArtistSongs(artistName);
            if (songs == null)
                return Response(model);

            foreach (string songJson in songs)
            {
                var song = (JObject)JsonConvert.DeserializeObject(songJson);
                int iTrackId;
                string previewUrl = "";
                string affiliateUrl = "";

                if (int.TryParse(song["TrackId"].ToString(), out iTrackId))
                {
                    previewUrl = _musicService.GetTrackPreviewUrl(iTrackId);
                    affiliateUrl = _musicService.GetTrackAffiliateUrl(iTrackId);
                }

                model.Add(new {
                    Id = song["RemoteEntityId"].ToString(),
                    Name = song["Name"].ToString(),
                    SeName = song["RemoteEntityId"].ToString(),
                    ImageUrl = song["ImageUrl"].ToString(),
                    PreviewUrl = previewUrl,
                    ForeignId = song["ForeignId"].ToString(),
                    TrackId = song["TrackId"].ToString(),
                    AffiliateUrl = affiliateUrl,
                    RemoteSong = true
                });
            }
            return Response(model);

        }
        [Route("getartistsongsbyid/{artistPageId:int}")]
        public IHttpActionResult GetArtistSongsByArtistPage(int artistPageId)
        {
            //check if artist page exists
            var artistPage = _artistPageService.Get(artistPageId);
            if (artistPage == null)
                return null;
            var model = new List<object>();
            foreach (var song in artistPage.Songs)
            {
                var product = _productService.GetProductById(song.AssociatedProductId);
                if (product == null) //no associated product. may be a remote artist..what say?
                    continue;

                var download = _downloadService.GetDownloadById(product.SampleDownloadId);

                var songPictures = _pictureService.GetEntityPictures<Song>(song.Id);
                model.Add(new {
                    Id = song.Id,
                    Name = song.Name,
                    SeName = song.GetPermalink(),
                    ImageUrl = songPictures.Count > 0 ? _pictureService.GetPictureUrl(songPictures.First()) : "",
                    PreviewUrl = song.PreviewUrl,
                    TrackId = song.Id,
                    DownloadId = download == null ? 0 : download.Id,
                    AssociatedProductId = product.Id,
                    RemoteSong = false
                });
            }
            return Response(model);

        }
        [HttpGet]
        [Route("getartistsongpreviewurl/{trackId}")]
        public IHttpActionResult GetArtistSongPreviewUrl(string trackId)
        {
            int iTrackId;
            if (int.TryParse(trackId, out iTrackId))
            {
                var previewUrl = _musicService.GetTrackPreviewUrl(iTrackId);
                return Response(new { Success = true, PreviewUrl = previewUrl });
            }
            return Response(new { Success = false, Message = "Invalid Track Id" });
        }

        [HttpGet]
        [Route("getpurchasedsongs/{artistPageId:int}")]
        public IHttpActionResult GetPurchasedSongs(int artistPageId, int Count = 15, int Page = 1)
        {
            /*var artistPage = _artistPageService.Get(artistPageId);
            var modelList = new List<object>();
            int totalPages = 0, totalSales = 0;
            decimal totalGross = 0, totalFee = 0, totalNet = 0;
            if (artistPage != null && CanDelete(artistPage))
            {
                var customerOrderItems = _orderService.GetAllOrderItems(null, null, null, null, null, PaymentStatus.Paid, null, true);
                var artistPageSongProductIds = artistPage.Songs.Select(x => x.AssociatedProductId);
                //let's get the purchased items
                var purchasedItems = customerOrderItems.Where(x => artistPageSongProductIds.Contains(x.Product.Id));

                //summary
                totalSales = purchasedItems.Count();
                totalPages = Convert.ToInt32(Math.Ceiling((decimal)purchasedItems.Count() / Count));
                totalGross = purchasedItems.Sum(x => _taxSettings.PricesIncludeTax ? x.PriceInclTax : x.PriceExclTax);
                totalFee = (totalGross * _mobSocialSettings.PurchasedSongFeePercentage) / 100;
                totalNet = totalGross - totalFee;

                //pagination
                purchasedItems = purchasedItems.Skip(Count * (Page - 1)).Take(Count);

                foreach (var pi in purchasedItems)
                {
                    var song = _songService.GetSongByProductId(pi.Product.Id);
                    var grossPrice = _taxSettings.PricesIncludeTax ? pi.PriceInclTax : pi.PriceExclTax;
                    var feeAmount = (grossPrice * _mobSocialSettings.PurchasedSongFeePercentage) / 100;
                    var netPrice = grossPrice - feeAmount;

                    //add new object to collection
                    modelList.Add(new {
                        SongName = pi.Product.Name,
                        PurchasedOn = _dateTimeHelper.ConvertToUserTime(pi.Order.CreatedOnUtc, DateTimeKind.Utc).ToString(),
                        OrderId = pi.Order.Id,
                        PreviewUrl = song.PreviewUrl,
                        SellPrice = _priceFormatter.FormatPrice(grossPrice, true, _workContext.WorkingCurrency),
                        FeeAmount = _priceFormatter.FormatPrice(feeAmount, true, _workContext.WorkingCurrency),
                        NetPrice = _priceFormatter.FormatPrice(netPrice, true, _workContext.WorkingCurrency),
                        SeName = song.GetPermalink()
                    });
                }
            }
            var model = new {
                Songs = modelList,
                Page = Page,
                Count = Count,
                TotalPages = totalPages,
                TotalSellPrice = _priceFormatter.FormatPrice(totalGross, true, _workContext.WorkingCurrency),
                TotalFeeAmount = _priceFormatter.FormatPrice(totalFee, true, _workContext.WorkingCurrency),
                TotalNetPrice = _priceFormatter.FormatPrice(totalNet, true, _workContext.WorkingCurrency),
                TotalSales = totalSales
            };
            return Response(model);*/
            //todo: implement
            throw new NotImplementedException();
            
        }


        [HttpGet]
        [Route("search")]
        public IHttpActionResult Search(string term, int Count = 15, int Page = 1, bool SearchDescriptions = false)
        {
            //we search for artists both in our database as well as the remote api
            var model = new List<object>();

            //first let's search our database
            var dbArtists = _artistPageService.SearchArtists(term, Count, Page, SearchDescriptions);

            //first add db artists
            foreach (var dba in dbArtists)
            {
                var imageUrl = "";
                var pictures = dba.GetPictures();
                if (pictures.Count > 0)
                    imageUrl = _pictureService.GetPictureUrl(pictures.First());

                model.Add(new {
                    Name = dba.Name,
                    Id = dba.Id,
                    ImageUrl = imageUrl,
                    ShortDescription = dba.ShortDescription,
                    SeName = dba.GetPermalink(),
                    RemoteArtist = false
                });
            }

            //do we need more records to show?
            if (dbArtists.Count() < Count)
            {
                //we need more records to show. lets go remote and import some records from there
                var remoteArtists = _artistPageApiService.SearchArtists(term, Count - dbArtists.Count());
                if (remoteArtists != null)
                {
                    var remoteArtistsDeserialized = new List<JObject>();
                    foreach (string raItem in remoteArtists)
                    {
                        remoteArtistsDeserialized.Add((JObject)JsonConvert.DeserializeObject(raItem));
                    }

                    var remoteArtistIds = remoteArtistsDeserialized.Select(x => x["RemoteEntityId"].ToString()).ToList();

                    //filter out the results which are already in our result set
                    remoteArtistIds = remoteArtistIds.Except(dbArtists.Select(x => x.RemoteEntityId)).ToList();

                    //now add remote artists if any
                    foreach (string raid in remoteArtistIds)
                    {
                        var artistJson = remoteArtistsDeserialized.First(x => x["RemoteEntityId"].ToString() == raid).ToString();
                        var artist = (JObject)JsonConvert.DeserializeObject(artistJson);
                        model.Add(new {
                            Name = artist["Name"].ToString(),
                            Id = raid,
                            ImageUrl = artist["ImageUrl"].ToString(),
                            ShortDescription = artist["ShortDescription"].ToString(),
                            SeName = raid,
                            RemoteArtist = true
                        });

                    }
                }

            }

            return Response(model);
        }

        /// <summary>
        /// Generic method for all inline updates
        /// </summary>
        [HttpPost]
        [Route("updateartistdata")]
        [Authorize]
        public IHttpActionResult UpdateArtistData(FormCollection parameters)
        {
            var IdStr = parameters["id"];
            int Id;
            if (int.TryParse(IdStr, out Id))
            {
                var artistPage = _artistPageService.Get(Id);

                if (CanEdit(artistPage))
                {
                    //find the key that'll be updated
                    var key = parameters["key"];
                    var value = parameters["value"];
                    switch (key)
                    {
                        case "Name":
                            artistPage.Name = value;
                            break;
                        case "Description":
                            artistPage.Biography = value;
                            break;
                        case "ShortDescription":
                            artistPage.ShortDescription = value;
                            break;
                        case "Gender":
                            artistPage.Gender = value;
                            break;
                        case "HomeTown":
                            artistPage.HomeTown = value;
                            break;
                    }
                    _artistPageService.Update(artistPage);
                    return Response(new { success = true });
                }
                else
                {
                    return Response(new { success = false, message = "Unauthorized" });

                }
            }
            else
            {
                return Response(new { success = false, message = "Invalid artist" });
            }

        }


        [HttpPost]
        [Authorize]
        [Route("myartistpages")]
        public IHttpActionResult MyArtistPages(string search = "", int count = 15, int page = 1)
        {
            int totalPages;
            var artistPages = _artistPageService.GetArtistPagesByPageOwner(ApplicationContext.Current.CurrentUser.Id, out totalPages, search, count, page, ApplicationContext.Current.CurrentUser.IsAdministrator() /*orphan pages for administrator*/);


            var dataList = new List<object>();

            foreach (var artist in artistPages)
            {
                var pictures = artist.GetPictures();
                var tmodel = new {
                    Description = artist.Biography,
                    DateOfBirth = artist.DateOfBirth,
                    City = artist.HomeTown,
                    Gender = artist.Gender,
                    Name = artist.Name,
                    ShortDescription = artist.ShortDescription,
                    HomeTown = artist.HomeTown,
                    RemoteEntityId = artist.RemoteEntityId,
                    RemoteSourceName = artist.RemoteSourceName,
                    Id = artist.Id,
                    SeName = artist.GetPermalink(),
                    MainPictureUrl =pictures.Any() ? _pictureService.GetPictureUrl(pictures.First()) : ""
                };


                dataList.Add(tmodel);
            }
            var model = new {
                Artists = dataList,
                TotalPages = totalPages,
                Count = count,
                Page = page
            };

            return Response(model);

        }

        /// <summary>
        /// Gets all pages managed by logged in user
        /// </summary>
        [HttpGet]
        [Authorize]
        [Route("getpagesasmanager")]
        public IHttpActionResult GetPagesAsManager()
        {
            var artistPages = _artistPageManagerService.GetPagesAsManager(ApplicationContext.Current.CurrentUser.Id);
            var dataList = new List<object>();

            foreach (var artist in artistPages)
            {
                var pictures = artist.GetPictures();
                var tmodel = new {
                    Description = artist.Biography,
                    DateOfBirth = artist.DateOfBirth,
                    City = artist.HomeTown,
                    Gender = artist.Gender,
                    Name = artist.Name,
                    ShortDescription = artist.ShortDescription,
                    HomeTown = artist.HomeTown,
                    RemoteEntityId = artist.RemoteEntityId,
                    RemoteSourceName = artist.RemoteSourceName,
                    Id = artist.Id,
                    SeName = artist.GetPermalink(),
                    MainPictureUrl = pictures.Any() ? _pictureService.GetPictureUrl(pictures.First()) : ""
                };


                dataList.Add(tmodel);
            }
            var model = new {
                Artists = dataList,
            };

            return Response(model);
        }

        /// <summary>
        /// Loads the artist editor page
        /// </summary>
        [Authorize]
        [Route("getentity/{artistPageId:int}")]
        public IHttpActionResult Editor(int artistPageId = 0)
        {
            ArtistPageModel model = null;
            if (artistPageId != 0)
            {
                var artist = _artistPageService.Get(artistPageId);
                //can the current user edit this page?
                if (CanEdit(artist))
                {
                    model = new ArtistPageModel() {
                        Description = artist.Biography,
                        DateOfBirth = artist.DateOfBirth,
                        Gender = artist.Gender,
                        Name = artist.Name,
                        ShortDescription = artist.ShortDescription,
                        RemoteEntityId = artist.RemoteEntityId,
                        RemoteSourceName = artist.RemoteSourceName,
                        HomeTown = artist.HomeTown
                    };

                }
                else
                {
                    return NotFound();
                }

            }
            else
            {
                model = new ArtistPageModel();
            }

            return Response(model);

        }


        /// <summary>
        /// Checks if a particular artist name is available
        /// </summary>
        [HttpGet]
        [Route("getartistnameavailability/{name}")]
        public IHttpActionResult GetArtistNameAvailability(string name)
        {
            /*
             * sends three parameters to client with json.
             * remoteArtist: specifies if remote artist is being sent if 'available' is true
             * artist: the artistJson response if 'available' is true
             * available: specifies if the name is 'available' or not
             */
            string artistJson;
            if (IsArtistPageNameAvailable(name, out artistJson))
            {

                if (artistJson == "")
                {
                    return Response(new { available = true, remoteArtist = false });
                }
                else
                {
                    return Response(new { available = true, remoteArtist = true, artist = artistJson });
                }

            }
            else
            {
                return Response(new { available = false });
            }
        }


        /// <summary>
        /// Saves the artist pages. Performs insertion
        /// </summary>
        [HttpPost]
        [Route("saveartist")]
        public IHttpActionResult SaveArtist(ArtistPageModel model)
        {

            if (!ModelState.IsValid)
            {
                VerboseReporter.ReportError("Invalid data submitted. Please check all fields and try again.", "save_artist");
                return RespondFailure();
            }

            if (!ApplicationContext.Current.CurrentUser.IsRegistered())
            {
                VerboseReporter.ReportError("Unauthorized access", "save_artist");
                return RespondFailure();
            }
         
            //check to see if artist name already exists
            string artistJson;
            if (IsArtistPageNameAvailable(model.Name, out artistJson))
            {
                var artistPage = new ArtistPage() {
                    PageOwnerId = ApplicationContext.Current.CurrentUser.Id,
                    Biography = model.Description,
                    Name = model.Name,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    HomeTown = model.HomeTown,
                    RemoteEntityId = model.RemoteEntityId,
                    RemoteSourceName = model.RemoteSourceName,
                    ShortDescription = model.ShortDescription
                };

                _artistPageService.Insert(artistPage);

                if (artistJson != "")
                {
                    //we can now download the image from the server and store it on our own server
                    //use the json we retrieved earlier
                    var jObject = (JObject)JsonConvert.DeserializeObject(artistJson);

                    if (!string.IsNullOrEmpty(jObject["ImageUrl"].ToString()))
                    {
                        var imageUrl = jObject["ImageUrl"].ToString();
                        var imageBytes = HttpHelper.ExecuteGet(imageUrl);
                        var fileExtension = Path.GetExtension(imageUrl);
                        if (!string.IsNullOrEmpty(fileExtension))
                            fileExtension = fileExtension.ToLowerInvariant();

                        var contentType = PictureUtility.GetContentType(fileExtension);
                        var picture = new Media()
                        {
                            Binary = imageBytes,
                            Name = model.Name,
                            MimeType = contentType
                        };
                        _pictureService.WritePictureBytes(picture, _mediaSettings.PictureSaveLocation);
                        //relate both page and picture
                        _pictureService.AttachMediaToEntity(artistPage, picture);
                    }

                }

                return Response(new {
                    Success = true,
                    RedirectTo = Url.Route("ArtistPageUrl", new RouteValueDictionary()
                    {
                        { "SeName" , artistPage.GetPermalink()}
                    })
                });

            }
            else
            {
                return Response(new {
                    Success = false,
                    Message = "DuplicateName"
                });
            }

        }

        [HttpPost]
        [Route("geteligiblemanagers")]
        public IHttpActionResult GetEligibleManagers(int artistPageId)
        {
            //first lets find out if current user can actually play around with this page
            var artistPage = _artistPageService.Get(artistPageId);
            var model = new List<object>();
            if (CanDelete(artistPage))
            {
                //so the current user is actually admin or the page owner. let's find friends
                //only friends can become page managers

                var friends = _friendService.GetCustomerFriends(ApplicationContext.Current.CurrentUser.Id);

                foreach (var friend in friends)
                {
                    var friendId = (friend.FromCustomerId == ApplicationContext.Current.CurrentUser.Id) ? friend.ToCustomerId : friend.FromCustomerId;
                    var friendCustomer = _userService.Get(friendId);

                    if (friendCustomer == null)
                        continue;

                    var friendThumbnailUrl = _pictureService.GetPictureUrl(
                            friendCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                            100,
                            true);

                    model.Add(new {
                        CustomerDisplayName = friendCustomer.GetFullName().ToTitleCase(),
                        ProfileUrl = Url.Route("CustomerProfileUrl", new RouteValueDictionary()
                        {
                            { "SeName" , friendCustomer.GetSeName(0) } 
                        }),
                        ProfileImageUrl = friendThumbnailUrl,
                        Id = friendId
                    });
                }

                return Response(model);

            }
            return Response(model);
        }

        [HttpPost]
        [Route("getpagemanagers")]
        public IHttpActionResult GetPageManagers(int artistPageId)
        {
            //first lets find out if current user can actually play around with this page
            var artistPage = _artistPageService.Get(artistPageId);
            var model = new List<object>();
            if (CanDelete(artistPage))
            {
                //so the current user is actually admin or the page owner. let's find managers

                var managers = _artistPageManagerService.GetPageManagers(artistPageId);

                foreach (var manager in managers)
                {

                    var customer = _userService.Get(manager.CustomerId);

                    if (customer == null)
                        continue;

                    var customerThumbnailUrl = _pictureService.GetPictureUrl(
                            customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                            100,
                            true);

                    model.Add(new {
                        CustomerDisplayName = customer.GetFullName().ToTitleCase(),
                        ProfileUrl =Url.Route("CustomerProfileUrl", new RouteValueDictionary()
                        {
                            { "SeName" , customer.GetSeName(0) } 
                        }),
                        ProfileImageUrl = customerThumbnailUrl,
                        Id = customer.Id
                    });
                }

                return Response(model);

            }
            return Response(model);
        }

        [HttpPost]
        [Route("savepagemanager")]
        public IHttpActionResult SavePageManager(int artistPageId, int customerId)
        {
            //let's perform a few checks before saving the page manager
            //1. does the artist page exist and is the current user eligible to add manager?
            var artistPage = _artistPageService.Get(artistPageId);
            if (artistPage != null && CanDelete(artistPage))
            {
                //2. does the customer really exist?
                var customer = _userService.Get(customerId);
                if (customer != null)
                {
                    //3. is the customer already a page manager
                    if (_artistPageManagerService.IsPageManager(artistPageId, customerId))
                    {
                        VerboseReporter.ReportError("The user is already the page manager for this page", "save_page_manager");
                        return RespondFailure();
                    }
                    //enough checks...save the new manager now
                    _artistPageManagerService.AddPageManager(new ArtistPageManager() {
                        ArtistPageId = artistPageId,
                        CustomerId = customerId,
                    });

                    return RespondSuccess();
                }
                else
                {
                    VerboseReporter.ReportError("The user doesn't exists", "save_page_manager");
                    return RespondFailure();
                }
            }
            else
            {
                VerboseReporter.ReportError("Unauthorized", "save_page_manager");
                return RespondFailure();
            }
        }

        [HttpPost]
        [Route("deletepagemanager")]
        public IHttpActionResult DeletePageManager(int artistPageId, int customerId)
        {
            //let's perform a few checks before saving the page manager
            //1. does the artist page exist and is the current user eligible to add manager?
            var artistPage = _artistPageService.Get(artistPageId);
            if (artistPage != null && CanDelete(artistPage))
            {
                //2. does the customer really exist?
                var customer = _userService.Get(customerId);
                if (customer != null)
                {
                    _artistPageManagerService.DeletePageManager(artistPageId, customerId);

                    return Response(new { Success = true });
                }
                else
                {
                    return Response(new { Success = false, Message = "CustomerDoesNotExit" });
                }
            }
            else
            {
                return Response(new { Success = false, Message = "Unauthorized" });
            }
        }
        [HttpPost]
        [Route("deleteartistpage/{artistPageId:int}")]
        public IHttpActionResult DeleteArtistPage(int artistPageId)
        {
            var artist = _artistPageService.Get(artistPageId);
            if (CanDelete(artist))
            {
                //the logged in user can delete the page. lets delete the associated things now
                var pictures = artist.GetPictures();
                foreach (var picture in pictures)
                {
                    _pictureService.DetachMediaFromEntity<ArtistPage>(artistPageId, picture.Id);
                    _pictureService.Delete(picture);
                }

                _artistPageService.Delete(artist);
                return Response(new { Success = true });
            }
            else
            {
                return Response(new { Success = false, Message = "Unauthorized" });
            }
        }

        [HttpPost]
        public IHttpActionResult UploadPicture(int artistPageId, IEnumerable<HttpPostedFileBase> file)
        {

            //first get artist page
            var artistPage = _artistPageService.Get(artistPageId);
            if (!CanEdit(artistPage))
                return Response(new { Success = false, Message = "Unauthorized" });

            var files = file.ToList();
            var newImageUrl = "";
            foreach (var fi in files)
            {
                Stream stream = null;
                var fileName = "";
                var contentType = "";

                if (file == null)
                    throw new ArgumentException("No file uploaded");

                stream = fi.InputStream;
                fileName = Path.GetFileName(fi.FileName);
                contentType = fi.ContentType;

                var fileBinary = new byte[stream.Length];
                stream.Read(fileBinary, 0, fileBinary.Length);

                var fileExtension = Path.GetExtension(fileName);
                if (!string.IsNullOrEmpty(fileExtension))
                    fileExtension = fileExtension.ToLowerInvariant();


                if (string.IsNullOrEmpty(contentType))
                {
                    contentType = PictureUtility.GetContentType(fileExtension);
                }

                var picture = new Media()
                {
                    Binary = fileBinary,
                    MimeType = contentType,
                    Name = fileName,
                    SystemName = Guid.NewGuid().ToString("n")
                };
                _pictureService.WritePictureBytes(picture, _mediaSettings.PictureSaveLocation);
          
                _pictureService.AttachMediaToEntity(artistPage, picture);
            
                newImageUrl = _pictureService.GetPictureUrl(picture.Id);

            }

            return Response(new { Success = true, Url = newImageUrl });
        }

        [HttpPost]
        public IHttpActionResult GetPaymentMethod(int artistPageId)
        {
            var artistPage = _artistPageService.Get(artistPageId);

            if (CanDelete(artistPage))
            {
                //the user can access payment method. let's fetch it.

                var paymentMethod = _artistPagePaymentService.GetPaymentMethod(artistPageId);
                if (paymentMethod != null)
                {
                    var model = new ArtistPagePaymentModel() {
                        AccountNumber = paymentMethod.AccountNumber,
                        Address = paymentMethod.Address,
                        BankName = paymentMethod.BankName,
                        City = paymentMethod.City,
                        Country = paymentMethod.Country,
                        PostalCode = paymentMethod.PostalCode,
                        PayableTo = paymentMethod.PayableTo,
                        PaymentTypeId = (int)paymentMethod.PaymentType,
                        PaypalEmail = paymentMethod.PaypalEmail,
                        RoutingNumber = paymentMethod.RoutingNumber,
                        ArtistPageId = paymentMethod.ArtistPageId,
                        Id = paymentMethod.Id
                    };
                    return Response(new { Success = true, PaymentMethod = model });
                }
                else
                {
                    var model = new ArtistPagePaymentModel() {
                        ArtistPageId = artistPageId,
                        PaymentTypeId = (int)ArtistPagePayment.PagePaymentType.Paypal
                    };
                    return Response(new { Success = true, PaymentMethod = model });
                }

            }
            else
            {
                return Response(new { Success = false, Message = "Unauthorized" });
            }
        }

        [HttpPost]
        public IHttpActionResult SavePaymentMethod(ArtistPagePaymentModel Model)
        {
            if (!ModelState.IsValid)
                return Response(new { Success = false, Message = "Invalid data submitted" });

            var artistPage = _artistPageService.Get(Model.ArtistPageId);
            if (CanEdit(artistPage))
            {
                var paymentMethod = _artistPagePaymentService.GetPaymentMethod(Model.ArtistPageId);
                if (paymentMethod == null)
                {
                    paymentMethod = new ArtistPagePayment();
                }
                //set the new values
                paymentMethod.AccountNumber = Model.AccountNumber;
                paymentMethod.Address = Model.Address;
                paymentMethod.ArtistPageId = Model.ArtistPageId;
                paymentMethod.BankName = Model.BankName;
                paymentMethod.City = Model.City;
                paymentMethod.Country = Model.Country;
                paymentMethod.PostalCode = Model.PostalCode;
                paymentMethod.PayableTo = Model.PayableTo;
                paymentMethod.PaymentType = (ArtistPagePayment.PagePaymentType)Model.PaymentTypeId;
                paymentMethod.PaypalEmail = Model.PaypalEmail;
                paymentMethod.RoutingNumber = Model.RoutingNumber;

                if (paymentMethod.Id == 0)
                    _artistPagePaymentService.InsertPaymentMethod(paymentMethod);
                else
                    _artistPagePaymentService.UpdatePaymentMethod(paymentMethod);

                return Response(new { Success = true });
            }
            else
            {
                return Response(new { Success = false, Message = "Unauthorized" });

            }
        }
        #endregion



        #region functions

        [NonAction]
        bool IsArtistPageNameAvailable(string name, out string artistJson)
        {
            artistJson = "";
            if (string.IsNullOrEmpty(name))
                return false;

            //first check if an artist page with this name exists in our database. If it does, game over. It's unavailable now.
            var artistPage = _artistPageService.GetArtistPageByName(name);
            if (artistPage != null)
                return false;

            //so it's not available in our database. let's now check if it's a remote api artist.
            //if it's a remote api artist, then user must be admin to create this artist page else it wont be available

            if (_artistPageApiService.DoesRemoteArtistExist(name))
            {
                if (ApplicationContext.Current.CurrentUser.IsAdministrator())
                {
                    artistJson = _artistPageApiService.GetRemoteArtist(name);
                    return true;
                }
                else
                {
                    //user is not admin so not available because only admin is allowed to create the remote artist page
                    return false;
                }
            }
            else
            {
                //remote name doesn't exists. so this is available.
                return true;
            }
        }


        /// <summary>
        /// Checks if current logged in user can actually edit the page
        /// </summary>
        /// <returns>True if editing is allowed. False otherwise</returns>
        [NonAction]
        bool CanEdit(ArtistPage ArtistPage)
        {
            if (ArtistPage == null)
                return false;
            return ApplicationContext.Current.CurrentUser.Id == ArtistPage.PageOwnerId //page owner
                || ApplicationContext.Current.CurrentUser.IsAdministrator() //administrator
                || _artistPageManagerService.IsPageManager(ArtistPage.Id, ApplicationContext.Current.CurrentUser.Id); //page manager
        }

        /// <summary>
        /// Checks if current logged in user can actually delete the page
        /// </summary>
        /// <returns>True if deletion is allowed. False otherwise</returns>
        [NonAction]
        bool CanDelete(ArtistPage artistPage)
        {
            if (artistPage == null)
                return false;
            return ApplicationContext.Current.CurrentUser.Id == artistPage.PageOwnerId //page owner
                || ApplicationContext.Current.CurrentUser.IsAdministrator(); //administrator
        }

        [NonAction]
        ArtistPage SaveRemoteArtistToDB(string artistJson)
        {
            if (string.IsNullOrEmpty(artistJson))
                return null;

            var artist = (JObject)JsonConvert.DeserializeObject(artistJson);
            var artistPage = new ArtistPage() {
                PageOwnerId = ApplicationContext.Current.CurrentUser.IsAdministrator() ? ApplicationContext.Current.CurrentUser.Id : 0,
                Biography = artist["Description"].ToString(),
                Name = artist["Name"].ToString(),
                Gender = artist["Gender"].ToString(),
                HomeTown = artist["HomeTown"].ToString(),
                RemoteEntityId = artist["RemoteEntityId"].ToString(),
                RemoteSourceName = artist["RemoteSourceName"].ToString(),
                ShortDescription = "",
            };

            _artistPageService.Insert(artistPage);

            //we can now download the image from the server and store it on our own server
            //use the json we retrieved earlier

            if (!string.IsNullOrEmpty(artist["ImageUrl"].ToString()))
            {
                var imageUrl = artist["ImageUrl"].ToString();
                var imageBytes = HttpHelper.ExecuteGet(imageUrl);
                if (imageBytes != null)
                {
                    var fileExtension = Path.GetExtension(imageUrl);
                    if (!String.IsNullOrEmpty(fileExtension))
                        fileExtension = fileExtension.ToLowerInvariant();

                    var contentType = PictureUtility.GetContentType(fileExtension);
                    var picture = new Media()
                    {
                        Binary = imageBytes,
                        MimeType = contentType,
                        Name = artistPage.Name
                    };
                    _pictureService.WritePictureBytes(picture, _mediaSettings.PictureSaveLocation);

                    _pictureService.AttachMediaToEntity(artistPage, picture);
                    
                }

            }
            return artistPage;
        }

        #endregion


    }
}
