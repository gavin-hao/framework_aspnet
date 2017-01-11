using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;
using Dongbo.AspNet.Identity.Extension;
using Dongbo.Identity.Security;
using Newtonsoft.Json;
using System.Web.Http.Cors;
namespace Dongbo.OAuth.Simple.ResourceServer.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class MeController : ApiController
    {
        public UserProfile Get()
        {
            
            var identity = User.Identity as ClaimsIdentity;
            if (null == identity) throw new NullReferenceException();

            var profile = new UserProfile();
            profile.Id = identity.FindFirstValue(ClaimTypes.NameIdentifier); //identity.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier).Value;
            profile.Name = identity.FindFirstValue(ClaimTypes.Name);
            profile.Email = identity.GetUserEmail();
            profile.Avatar = identity.FindFirstValue(ClaimTypes.Uri);
            return profile;//identity.Claims.Select(c => new { c.Type, c.Value });
        }
    }
    [JsonObject(MemberSerialization.OptIn)]
    public class UserProfile
    {
        [JsonProperty(PropertyName = "id")]
        public virtual string Id { get; set; }
        [JsonProperty(PropertyName = "name")]
        public virtual string Name { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [JsonProperty(PropertyName = "email")]
        public virtual string Email { get; set; }

        /// <summary>
        /// PhoneNumber for the user
        /// </summary>
        [JsonProperty(PropertyName = "phone")]
        public virtual string PhoneNumber { get; set; }

        /// <summary>
        /// user nickname
        /// </summary>
        [JsonProperty(PropertyName = "nickname")]
        public virtual string Nickname { get; set; }
        /// <summary>
        /// user' avatar url
        /// </summary>
        [JsonProperty(PropertyName = "avatar")]
        public virtual string Avatar { get; set; }
    }
}
