namespace prjtestAPI.Helpers
{
    public class EmailTemplateBuilder
    {
        public static string BuildInitPasswordEmail(string username, string link)
        {
            return $"""
                <p>{username} 您好，</p>
                <p>您已被建立帳號，請點擊以下連結設定登入密碼並啟用帳號：</p>
                <p><a href='{link}'>{link}</a></p>
                <p>此連結將於 24 小時後失效，請勿轉發。</p>
            """;
        }

        public static string BuildResetPasswordEmail(string username, string link)
        {
            return $"""
                <p>{username} 您好，</p>
                <p>請點擊以下連結重設您的登入密碼：</p>
                <p><a href='{link}'>{link}</a></p>
                <p>此連結將於 1 小時後失效，請勿轉發。</p>
            """;
        }
    }
}
