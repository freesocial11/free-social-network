window.mobSocial.factory('conversationHub', ['$q', 'Hub', '$rootScope', 'signalREndPoint', 'conversationService', 'webClientService', 'globalApiEndPoint', '$timeout', function ($q, Hub, $rootScope, signalREndPoint, conversationService, webClientService, globalApiEndPoint, $timeout) {
    //chat/conversation configurations
    var scrollToBottom = function () {
        $timeout(function() {
                var scrollHeight = angular.element(".chat-scroll .conversation")[0].scrollHeight;
                angular.element(".chat-scroll .conversation").animate({ scrollTop: scrollHeight });
            },
            0);
    }

    var markChatOpen = function (userId) {
        var opened = false;
        var cu = $rootScope.CurrentUser;
        for (var i = 0; i < $rootScope.Chat.openChats.length; i++) {
            var chat = $rootScope.Chat.openChats[i].conversation;
            var canOpen = ((chat.ReceiverId == userId && chat.UserId == cu.Id) ||
                    (chat.ReceiverId == cu.Id && chat.UserId == userId)) &&
                chat.ReceiverType == "User";

            if (canOpen) {
                $rootScope.Chat.activeChat = $rootScope.Chat.openChats[i];
                $rootScope.Chat.activeChat.toUserId = userId;
                opened = true;
                scrollToBottom();
            }
        }
        return opened;
    }
    
    var hub = new Hub('conversation',
    {
        rootPath: signalREndPoint,
        listeners: {
            'conversationReply': function (reply, conversationId) {
                $timeout(function () {
                    var chat = $rootScope.Chat.getChat(reply.UserId, conversationId);
                    if (chat) {
                        chat.conversation.ConversationReplies.push(reply);
                    }
                    $rootScope.$apply();
                    scrollToBottom();
                }, 0);
            },
            'notifyTyping': function(conversationId, userId, typing) {
                var chat = $rootScope.Chat.activeChat;
                if (chat.conversation.ConversationId == conversationId) {
                    $timeout(function () {
                        chat.activeTypingUserId = typing ? userId : 0;
                        $rootScope.$apply();
                        scrollToBottom();
                    }, 0);
                }
            }
        },
        methods: ['postReply', 'markRead', 'notifyTyping']
    });

    $rootScope.Chat = {
        openChats: [],
        activeChat: null,
        isOpen: function() {
            return $rootScope.Chat.activeChat != null;
        },
        isActiveChat: function (userId) {
            if (!$rootScope.Chat.activeChat)
                return false;
            var chat = $rootScope.Chat.activeChat.conversation;
            if (!chat)
                return false;
            var canOpen = (chat.ReceiverId == userId || chat.UserId == userId) && chat.ReceiverType == "User";
            return canOpen;
        },
        getChat: function (userId, conversationId) {
            var cu = $rootScope.CurrentUser;
            for (var i = 0; i < $rootScope.Chat.openChats.length; i++) {
                var chat = $rootScope.Chat.openChats[i].conversation;
                var canOpen = ((chat.ReceiverId == userId && chat.UserId == cu.Id) ||
                        (chat.ReceiverId == cu.Id && chat.UserId == userId)) &&
                    chat.ReceiverType == "User";

                if (canOpen) {
                    return $rootScope.Chat.openChats[i];
                } else {
                    if (chat.ConversationId == conversationId)
                        return $rootScope.Chat.openChats[i];
                }
            }
            return null;
        },
        loadChat: function (userId, page, callback) {
            conversationService.get(userId, page,
                    function (response) {
                        if (response.Success) {
                            var chat = $rootScope.Chat.getChat(userId);
                            if (!chat) {
                                $rootScope.Chat.openChats.push({
                                    conversation: response.ResponseData.Conversation || [],
                                    replyText: '',
                                    loadedPage: page,
                                    userId: userId
                                });
                                markChatOpen(userId);
                                if (callback)
                                    callback();
                                scrollToBottom();
                            } else {
                                var replies = response.ResponseData.Conversation.ConversationReplies;
                                for (var i = replies.length; i >= 0; i--)
                                    chat.conversation.ConversationReplies.unshift(replies[i]);
                            }
                            
                        }
                    });
        },
        chatWith: function(userId, callback) {
            if (!markChatOpen(userId)) {
                this.loadChat(userId, 1, callback);
            } else {
                if (callback)
                    callback();
                scrollToBottom();
            }
        },
        sendReply: function () {
            hub.postReply($rootScope.Chat.activeChat.toUserId, $rootScope.Chat.activeChat.replyText);
            $timeout(function() {
                    $rootScope.Chat.activeChat.replyText = "";
                    $rootScope.$apply();
                },
                0);

        },
        //get online friends
        loadOnlineFriends: function() {
            var url = globalApiEndPoint + "/friends";
            webClientService.get(url + "/get",
                null,
                function(response) {
                    if (response.Success) {
                        $rootScope.CurrentUser.Friends = response.ResponseData.Friends;
                    }
                });
        },
        markConversationRead: function(conversationId) {
            hub.markRead(conversationId);
        }
    };

    $rootScope.$watch("Chat.activeChat.replyText",
        function (newValue, oldValue) {
            var activeChat = $rootScope.Chat.activeChat;
            if (!activeChat) {
                return;
            }
            if (newValue == oldValue || oldValue == undefined) {
                return;
            }
            
            //some change has been done, we'll notify it or not depends on our previous notification
            var now = new Date();
            var nowSeconds = Math.round(now.getTime() / 1000);
            var prevSeconds = activeChat.lastNotificationSeconds || 0;
            var prevStatusTyping = activeChat.lastTypingStatus;

            var delay = nowSeconds - prevSeconds;
            //we'll send notification only after 2 seconds wait
            if (!prevStatusTyping && delay > 2) {
                //notify typing
                hub.notifyTyping(activeChat.conversation.ConversationId, true);
                activeChat.lastNotificationSeconds = nowSeconds;
                activeChat.lastTypingStatus = true;
                return;
            }
            //denotify only after 5 seconds
            if (prevStatusTyping && delay > 5) {
                //notify typing
                hub.notifyTyping(activeChat.conversation.ConversationId, false);
                activeChat.lastNotificationSeconds = nowSeconds;
                activeChat.lastTypingStatus = false;
                return;
            }
        });

    return this;
}]);