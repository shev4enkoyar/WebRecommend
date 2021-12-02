namespace WebRecommend
{
    public class WebConst
    {
        public static string AdminRole = "admin";
        public static string UserRole = "user";
        public static string BannedUser = "banned";

        public enum SortState
        {
            RatingAsc,
            RatingDesc,
            DateAsc,
            DateDesc
        }
    }
}
