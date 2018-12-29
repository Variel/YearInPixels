using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Variel.Web.Authentication;
using Variel.Web.Session;
using YearInPixels.Models.Data;
using YearInPixels.Models.Response;

namespace YearInPixels.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthenticationProviderFactory<Account> _authFactory;
        private readonly SessionService<Account> _session;

        public AuthController(IAuthenticationProviderFactory<Account> authFactory, SessionService<Account> session)
        {
            _authFactory = authFactory;
            _session = session;
        }

        public async Task<IActionResult> Login(string email, string password)
        {
            var provider =_authFactory.Create(AuthenticationProviders.Self);
            var account = await provider.AuthenticateAsync(email, password);

            if (account == null)
                return StatusCode(401, new ErrorResponseModel("아이디 혹은 비밀번호를 확인하세요"));

            await _session.LoginAsync(account);

            return Ok();
        }

        public async Task<IActionResult> Join(string email, string password, string name)
        {
            var provider = _authFactory.Create(AuthenticationProviders.Self);
            if (!provider.Check(email))
                return StatusCode(409, new ErrorResponseModel("이미 가입 된 이메일입니다"));

            await provider.CreateAccountAsync(new Account
            {
                Email = email,
                Name = name
            }, email, password);

            return Ok();
        }
    }
}
