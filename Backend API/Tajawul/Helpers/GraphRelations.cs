namespace Tajawul.Helpers
{
    public static class GraphRelations
    {
        public static class User
        {
            public const string Visited = "VISITED";
            public const string Followed = "FOLLOWED";
            public const string FollowedUser = "FOLLOWED_USER";
            public const string Reviewed = "REVIEWED";
            public const string Wished = "WISHED";
            public const string FavoritedDestination = "Favorited";
            public const string InterestedIn = "INTERESTED_IN";
            public const string SearchedFor = "SEARCHED_FOR";
            public const string Shared = "SHARE";
            public const string Created = "CREATED";
            public const string CreatedTrip = "CREATED";
            public const string React = "REACT";
            public const string Attend = "ATTEND";
            public const string Speak = "SPEAKED";
            public const string Follow = "FOLLOW";
            public const string Edited = "EDITED";
            public const string Recommended = "RECOMMENDED";
            public const string Similar = "SIMILAR";
            public const string PreferedActivity = "PREFERED_ACTIVITY";
            public const string PreferedStyle = "PREFERED_STYLE";
            public const string HasMaritalStatus = "HAS_MARITAL_STATUS";
            public const string HadGender = "HAD_GENDER";
            public const string HadTag = "HAD_TAG";
            public const string HadPriceRange = "HAD_PRICE_RANGE";
            public const string HadDestinationType= "HAD_DESTINATION_TYPE";
            public const string HadGroupSize = "HAD_GROUP_SIZE";
            public const string PreferedDuration = "PREFERED_DURATION";
            public const string LocatedIn = "LOCATED_IN";
            public const string Attended = "ATTENDED";


            public const string FavoritedTrip = "FAVORITED";
            public const string WishedTrip = "WICHED";
            public const string Cloned = "CLONED";
        }

        public static class Destination
        {
            public const string Created = "CREATED";
            public const string Shared = "SHARED";
            public const string Organized = "ORGANIZED";
            public const string Edited = "EDITED";
            public const string HadPriceRange = "HAD_PRICE_RANGE";
            public const string HadActivity = "HAD_ACTIVITY";
            public const string PreferedGroupSize = "PREFERED_GROUP_SIZE";
            public const string HadType = "HAD_TYPE";
            public const string LocatedIn = "LOCATED_IN";
            public const string Similar = "SIMILAR";
            public const string HadTag = "HAD_TAG";
            public const string OpenAt = "OPEN_AT";
            public const string CloseAt = "CLOSE_AT";
        }

        public static class Trip
        {

            public const string HadPriceRange = "HAD_PRICE_RANGE";
            public const string HadVisibility = "HAD_VISIBILITY";
            public const string HadDuration = "HAD_DURATION";
            public const string Included = "INCLUDED";
            public const string HadTag = "HAD_TAG";
            public const string HadStatus = "CURRENT_STATUS";

        }

        public static class Event
        {
            public const string StartedOn = "STARTED_ON";
            public const string EndedOn = "ENDED_ON";
            public const string HadPriceRange = "HAD_PRICE_RANGE";
            public const string LocatedIn = "LOCATED_IN";
            public const string HadTag = "HAD_TAG";
            public const string HadStatus = "CURRENT_STATUS";

        }

        public static class Post
        {
            public const string Contained = "CONTAINED";
            public const string HasVisibility = "HAS_VISIBILITY";
            public const string HadStyle = "HAD_STYLE";
            public const string HadTag = "HAD_TAG";
        }

        public static class Comment
        {
            public const string CommentedOn = "COMMENTED_ON";
            public const string Replied = "REPLYED";
            public const string Upvoted = "UPVOTED";
            public const string Downvoted = "DOWNVOTED";
        }

        public static class PlaceOwner
        {
            public const string Managed = "MANAGED";
        }


        public static class Location
        {
            public const string LocatedIn = "LOCATED_IN";
        }
    }

}
