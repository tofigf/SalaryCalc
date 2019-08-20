using CalcSalaryApi.ApiDtos;
using CalcSalaryApi.Data.Repository.Interface;
using DataAccessLayer;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace CalcSalaryApi.Controllers.V1
{
    [RoutePrefix("api/login")]
    public class LoginController : ApiController
    {
        private readonly IAuthRepository _repo;
        public LoginController(IAuthRepository repo)
        {
            _repo = repo;
        }
        [HttpPost]
        [Route("authenticate")]
        public async Task <IHttpActionResult> Authenticate([FromBody] LoginDto login)
        {
           
            if (await _repo.UserExists(login.Email, login.Password))
                return StatusCode(HttpStatusCode.Unauthorized);
            if (login == null)
                return Unauthorized();

            User userFromRepo = await _repo.Login(login.Email.ToLower(), login.Password);

            //IHttpActionResult response;
            //HttpResponseMessage responseMsg = new HttpResponseMessage();
            //bool isUsernamePasswordValid = false;

            if (userFromRepo == null)
                return StatusCode(HttpStatusCode.NotFound);
           
                string token = createToken(userFromRepo.Id);
                //return the token
                return Ok(new { token ,userFromRepo.Id,userFromRepo.UserName});
            
            //else
            //{
            //    // if credentials are not valid send unauthorized status code in response
            //    userFromRepo.responseMsg.StatusCode = HttpStatusCode.Unauthorized;
            //    response = ResponseMessage(loginResponse.responseMsg);
            //    return response;
            
        }

        private string createToken(int? id)
        {
            //Set issued at date
            DateTime issuedAt = DateTime.UtcNow;
            //set the time when it expires
            DateTime expires = DateTime.UtcNow.AddDays(7);

            var tokenHandler = new JwtSecurityTokenHandler();

            //create a identity and add claims to the user which we want to log in
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, id.ToString())
           
            });

            const string sec = "401b09eab3c013d4ca54922bb802bec8fd5318192b0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1081c39b740293f765eae731f5a65ed1";
            var now = DateTime.UtcNow;
            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(sec));
            var signingCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(securityKey, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256Signature);


            //create the jwt
            var token =
                (JwtSecurityToken)
                    tokenHandler.CreateJwtSecurityToken(issuer: "https://localhost:44302", audience: "https://localhost:44302",
                        subject: claimsIdentity, notBefore: issuedAt, expires: expires, signingCredentials: signingCredentials);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }
    }
}
