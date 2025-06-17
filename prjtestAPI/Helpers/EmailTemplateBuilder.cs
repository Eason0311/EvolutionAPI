using System.Text;

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

        public static string BuildOrderDetailEmail(
           string username,
           string transactionId,
           int orderId,
           IEnumerable<(string ProductName, int Quantity, decimal UnitPrice, decimal TotalPrice)> details,
           decimal totalAmount,
           DateTime purchaseTime)
        {
            // 先組 <tr> 內容
            var rows = new StringBuilder();
            foreach (var item in details)
            {
                rows.AppendLine($"""
                    <tr>
                      <td>{item.ProductName}</td>
                      <td align="center">{item.Quantity}</td>
                      <td align="right">NT${item.UnitPrice:F2}</td>
                      <td align="right">NT${item.TotalPrice:F2}</td>
                    </tr>
                    """);
            }

            // 回傳完整 HTML
            return $"""
                <p>{username} 您好，</p>
                <p>感謝您的消費！以下是您的訂單明細：</p>

                <p>交易編號：{transactionId}<br/>
                訂單編號：#{orderId}<br/>
                購買時間：{purchaseTime:yyyy/MM/dd HH:mm}</p>

                <table border="1" cellpadding="5" cellspacing="0" style="border-collapse:collapse;">
                  <thead>
                    <tr>
                      <th>課程名稱</th>
                      <th>數量</th>
                      <th>單價</th>
                      <th>小計</th>
                    </tr>
                  </thead>
                  <tbody>
                    {rows}
                  </tbody>
                </table>

                <p><strong>總金額：NT${totalAmount:F2}</strong></p>

                <p>祝您學習愉快！</p>
                """;
        }
    }
}
