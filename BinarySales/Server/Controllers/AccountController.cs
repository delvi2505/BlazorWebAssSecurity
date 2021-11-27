using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BinarySales.Shared.Models;
using BinarySales.Shared.Models.Entities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BinarySales.Shared;
using BinarySales.Server.DB_Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;

namespace BinarySales.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
        Roles = "USUARIOMAESTRO,ADMINISTRADOR")]
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AccountController(ApplicationDbContext db,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration)
            {
                this.db = db;
                _userManager = userManager;
                _signInManager = signInManager;
                _configuration = configuration;
            }

        //Metodos

        [HttpPost("CreateUser")]
        public async Task<ActionResult> CreateUser([FromBody] Employee model)
        {
            try
            {
                model.DNI = model.DNI.ToUpper();
                model.Names = model.Names.ToUpper();
                model.LastNames = model.LastNames.ToUpper();
                model.Rol = model.Rol.ToUpper();
                model.Status = model.Status.ToUpper();
                model.Created = DateTime.UtcNow;

                var user = new IdentityUser
                {
                    UserName = model.DNI,
                    NormalizedUserName = model.DNI,
                    PhoneNumber = model.Phone,
                    Email = model.Email,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString("D")
                };
                if (user.Email != null)
                    user.NormalizedEmail = model.Email.ToUpper();

                var result = await _userManager.CreateAsync(user, model.DNI.ToUpper());
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                result = await _userManager.AddToRoleAsync(user, model.Rol);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                await db.Employees.AddAsync(model);
                await db.SaveChangesAsync();

                return Ok("ok");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("SaveUser")]
        public async Task<ActionResult> SaveUser([FromBody] Employee model)
        {
            try
            {
                var employee = db.Employees.FirstOrDefault( x => x.Id == model.Id);
                if(employee == null)
                {
                    return BadRequest("No se pudo encontrar un usuario");
                }
                string oldDni = employee.DNI;
                
                employee.DNI = model.DNI.ToUpper();
                employee.Names = model.Names.ToUpper();
                employee.LastNames = model.LastNames.ToUpper();
                employee.Rol = model.Rol.ToUpper();
                employee.Status = model.Status.ToUpper();
                employee.Phone = model.Phone;
                employee.Email = model.Email;

                var user = db.Users.FirstOrDefault(x => x.UserName == oldDni);
                if (user == null)
                {
                    return BadRequest("No se pudo encontrar un usuario de acceso");
                }

                user.UserName = model.DNI;
                user.NormalizedUserName = model.DNI;
                user.PhoneNumber = model.Phone;
                user.Email = model.Email;
                if (user.Email != null)
                    user.NormalizedEmail = employee.Email.ToUpper();

                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                var resultRole = await _userManager.RemoveFromRoleAsync(user, employee.Rol);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                resultRole = await _userManager.AddToRoleAsync(user, model.Rol);
                if (!resultRole.Succeeded)
                {
                    return BadRequest(resultRole.Errors);
                }

                await db.SaveChangesAsync();
                return Ok("ok");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("ChangePassword")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePassModel model)
        {
            try
            {
                if (model.NewPass != model.ConfirmPass)
                {
                    return BadRequest("No se pudo confirmar el nuevo password");
                }

                var user = db.Users.FirstOrDefault(x => x.UserName == model.UserName);
                if (user == null)
                {
                    return BadRequest("No se pudo encontrar un usuario de acceso");
                }

                var result = await _userManager.ChangePasswordAsync(user,model.CurrentPass,model.NewPass);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                return Ok("ok");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPut("ResetPassword")]
        public async Task<ActionResult> ResetPassword([FromBody] int id)
        {
            try
            {
                var employee = db.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    return BadRequest("No se pudo encontrar un usuario");
                }

                var user = db.Users.FirstOrDefault(x => x.UserName == employee.DNI);
                if (user == null)
                {
                    return BadRequest("No se pudo encontrar un usuario de acceso");
                }

                var result = await _userManager.RemovePasswordAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                result = await _userManager.AddPasswordAsync(user, user.UserName);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok("ok");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetUsers/{search}/{status}/{actualPage}")]
        public ActionResult GetUsers(string search, string status, int actualPage)
        {
            try
            {
                if (search == "TODO")
                    search = "";
                if (status == "TODO")
                    status = "";

                search = search.ToUpper();
                int allRegisters = db.Employees.Where(x => (x.Names.Contains(search) || x.LastNames.Contains(search) || x.DNI.Contains(search)) && (x.Status.Contains(status)) && (x.Rol != "USUARIOMAESTRO")).Count();
                if(allRegisters <= 0)
                {
                    return NotFound(Utilities.MSGNODATA);
                }

                IList<Employee> entities = db.Employees.Where(x => (x.Names.Contains(search) || x.LastNames.Contains(search) || x.DNI.Contains(search)) && (x.Status.Contains(status)) && (x.Rol != "USUARIOMAESTRO"))
                   .OrderByDescending(x => x.Id)
                   .Skip((actualPage - 1) * Utilities.REGISTERSPERPAGE)
                   .Take(Utilities.REGISTERSPERPAGE)
                   .ToList();
                ResponseForList response = new ResponseForList()
                {
                    EntitiesPricipal = JsonSerializer.Serialize(entities),
                    AllRegisters = allRegisters,
                    ActualPage = actualPage
                };
                return Ok(JsonSerializer.Serialize(response));

            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }


        }

        [HttpGet("GetUserById/{id}")]
        public ActionResult GetUserById(int id)
        {
            try
            {
                var employee = db.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    return BadRequest("No se pudo encontrar un usuario");
                }
                return Ok(JsonSerializer.Serialize(employee));
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login([FromBody] UserLogin userInfo)
        {
            var employee = db.Employees.FirstOrDefault(x => x.DNI == userInfo.UserName);

            if (employee != null || userInfo.UserName == "admin")
            {
                if(employee != null && employee.Status != "ACTIVO")
                {
                    return BadRequest(Utilities.MSGSUSPENDEDUSER);
                }

                var result = await _signInManager.PasswordSignInAsync(userInfo.UserName, userInfo.Password, isPersistent: false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return Ok(BuildToken(userInfo.UserName));
                }
                else
                {
                    return BadRequest(Utilities.MSGCREDENTIALSFAILS);
                }
            }
            else
            {
                return BadRequest(Utilities.MSGCREDENTIALSFAILS);
            }
            
        }

        [HttpDelete("DeleteUser/{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                var employee = db.Employees.FirstOrDefault(x => x.Id == id);
                if (employee == null)
                {
                    return BadRequest("No se pudo encontrar un usuario");
                }

                var user = db.Users.FirstOrDefault(x => x.UserName == employee.DNI);
                if (user == null)
                {
                    return BadRequest("No se pudo encontrar un usuario de acceso");
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                db.Remove(employee);
                
                await db.SaveChangesAsync();
                return Ok("ok");
            }
            catch (Exception e)
            {

                return BadRequest(e.Message);
            }
        }

        private string BuildToken(string username)
        {
            var user = db.Users.First(x => x.UserName == username);
            string role = db.UserRoles.First(x => x.UserId == user.Id).RoleId;
            string completeName = "USUARIO MAESTRO";
            int employeeId = 0;
            if (role != "USUARIOMAESTRO")
            {
                var employee = db.Employees.First(x => x.DNI == user.UserName);
                completeName = $"{employee.Names} {employee.LastNames}";
                employeeId = employee.Id;
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim("userNM", username),
                new Claim(ClaimTypes.Name, completeName),
                new Claim("userId", user.Id),
                new Claim("employeeId", employeeId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddHours(10);

            JwtSecurityToken token = new JwtSecurityToken(
               issuer: null,
               audience: null,
               claims: claims,
               expires: expiration,
               signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
