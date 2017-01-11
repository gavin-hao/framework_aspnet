using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.AspNet.Identity.Extension
{

    public class ApplicationUserValidator<TUser> : IIdentityValidator<TUser>
        where TUser : class, IUser<string>
    {
        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="manager"></param>
        public ApplicationUserValidator(ApplicationUserManager<TUser> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException("manager");
            }
            //AllowOnlyAlphanumericUserNames = true;
            Manager = manager;
        }

        /// <summary>
        ///     Only allow [A-Za-z0-9@_] in UserNames
        /// </summary>
        //public bool AllowOnlyAlphanumericUserNames { get; set; }

        /// <summary>
        ///     If set, enforces that emails are non empty, valid, and unique
        /// </summary>
        public bool RequireUniqueEmail { get; set; }

        public bool RequireUniquePhoneNumber { get; set; }

        public bool RequireUniqueNickname { get; set; }
        private ApplicationUserManager<TUser> Manager { get; set; }

        /// <summary>
        ///     Validates a user before saving
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public virtual async Task<IdentityResult> ValidateAsync(TUser item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item");
            }
            var errors = new List<string>();
            //await ValidateUserName(item, errors);
            if (RequireUniqueEmail)
            {
                await ValidateEmailAsync(item, errors);
            }
            if (RequireUniquePhoneNumber)
            {
                await ValidatePhoneNumber(item, errors);
            }
            if (RequireUniqueNickname)
            {
                await ValidateNicknameAsync(item, errors);
            }
            if (errors.Count > 0)
            {
                return IdentityResult.Failed(errors.ToArray());
            }
            return IdentityResult.Success;
        }

        private async Task ValidateUserName(TUser user, List<string> errors)
        {
            //if (string.IsNullOrWhiteSpace(user.UserName))
            //{
            //    errors.Add(String.Format(CultureInfo.CurrentCulture, "{0} Too Short", "Name"));
            //}
            //else
            //{
            //    var owner = await Manager.FindByNameAsync(user.UserName);
            //    if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            //    {
            //        errors.Add(String.Format(CultureInfo.CurrentCulture, "UserName {0} already taken", user.UserName));
            //    }
            //}
            var owner = await Manager.FindByNameAsync(user.UserName);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add("UserName already taken");
                //errors.Add(String.Format(CultureInfo.CurrentCulture, "UserName {0} already taken", user.UserName));
            }
        }

        private async Task ValidatePhoneNumber(TUser user, List<string> errors)
        {
            var phone = await Manager.GetPhoneNumberStore().GetPhoneNumberAsync(user);
            //var email = await Manager.GetPhoneNumberStore().GetPhoneNumberAsync(user);
            if (string.IsNullOrWhiteSpace(phone))
            {
                return;
            }
            //if (!phone.IsMobilePhone())
            //{
            //    errors.Add(String.Format(CultureInfo.CurrentCulture, "Invalid PhoneNumber {0} ", phone));
            //}
            var owner = await Manager.FindByPhoneNumberAsync(phone);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add("PhoneNumber already taken");
                //errors.Add(String.Format(CultureInfo.CurrentCulture, "PhoneNumber {0} already taken", phone));
            }
        }

        // make sure if email is not empty then must valid, and unique
        private async Task ValidateEmailAsync(TUser user, List<string> errors)
        {
            var email = await Manager.GetEmailStore().GetEmailAsync(user);
            if (string.IsNullOrWhiteSpace(email))
            {
                //errors.Add(String.Format(CultureInfo.CurrentCulture, "{0} Too Short", "Email"));
                return;
            }
            //try
            //{
            //    var m = new MailAddress(email);
            //}
            //catch (FormatException)
            //{
            //    errors.Add(String.Format(CultureInfo.CurrentCulture, "Invalid Email {0}", email));
            //    return;
            //}
            var owner = await Manager.FindByEmailAsync(email);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add("Email already taken");
                //errors.Add(String.Format(CultureInfo.CurrentCulture, "Email {0} already taken", email));
            }
        }
        private async Task ValidateNicknameAsync(TUser user, List<string> errors)
        {
            var nickname = await Manager.GetNicknameExtraStore().GetNicknameAsync(user);
            if (string.IsNullOrWhiteSpace(nickname))
            {
                //errors.Add(String.Format(CultureInfo.CurrentCulture, "{0} Too Short", "Email"));
                return;
            }

            var owner = await Manager.FindByNicknameAsync(nickname);
            if (owner != null && !EqualityComparer<string>.Default.Equals(owner.Id, user.Id))
            {
                errors.Add("Nickname already taken");
                //errors.Add(String.Format(CultureInfo.CurrentCulture, "Nickname {0} already taken", nickname));
            }
        }
    }
}
