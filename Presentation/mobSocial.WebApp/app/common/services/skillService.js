"use strict";

window.mobSocial.lazy.service("skillService", ["globalApiEndPoint", "webClientService", function (globalApiEndPoint, webClientService) {
    var apiEndPoint = globalApiEndPoint + "/skills";
    
    this.getUserSkills = function(userId, success, error)
    {
        webClientService.get(apiEndPoint + "/users/" + userId + "/get", null, success, error);
    }

    this.getSkill = function (skilId, success, error) {
        webClientService.get(apiEndPoint + "/get/" + skilId, null, success, error);
    }

    this.postSkill = function (skill, success, error) {
        webClientService.post(apiEndPoint + "/post", skill, success, error);
    }

    this.delete = function (skillId, success, error) {
        webClientService.delete(apiEndPoint + "/delete/" + skillId, null, success, error);
    }

    this.deleteUserSkill = function(userSkillId, success, error) {
        webClientService.delete(apiEndPoint + "/users/delete/" + userSkillId, null, success, error);
    }
}]);
