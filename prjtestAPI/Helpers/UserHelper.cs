using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace prjEvolutionAPI.Helpers
{
    public static class UserHelper
    {
        public static int? GetUserId(this ClaimsPrincipal user)
        {
            if (user == null)
                return null;

            // 先找 NameIdentifier，再找 "sub"
            var claim = user.FindFirst(ClaimTypes.NameIdentifier)
                        ?? user.FindFirst("sub");
            if (claim == null)
                return null;

            // 再嘗試把 claim.Value 轉成 int
            if (!int.TryParse(claim.Value, out var id))
                return null;

            return id;
        }
    }
}
