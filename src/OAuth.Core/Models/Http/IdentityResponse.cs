using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Dongbo.OAuth.Core.Models.Http
{
    public class IdentityResponseJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(IdentityResponse).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var claims = new List<KeyValuePair<string, string>>();

            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.PropertyName)
                {
                    var propertyName = reader.Value.ToString();

                    if (!reader.Read())
                    {
                        throw new JsonSerializationException("Unexpected end when reading IdentityResponse.");
                    }

                    // Skip until all comments are gone
                    while (reader.TokenType == JsonToken.Comment)
                    {
                        if (!reader.Read())
                        {
                            throw new JsonSerializationException("Unexpected end when reading IdentityResponse.");
                        }
                    }

                    switch (reader.TokenType)
                    {
                        default:
                            if (this.IsPrimitiveToken(reader.TokenType))
                            {
                                claims.Add(new KeyValuePair<string, string>(propertyName, reader.Value.ToString()));
                                break;
                            }

                            throw new JsonSerializationException("Unexpected token when reading value for {propertyName}: {reader.Value} ({reader.TokenType})");
                    }
                }
            }

            return new IdentityResponse(claims);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var obj = value as IdentityResponse;

            if (obj == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteStartObject();

            foreach (var claim in obj)
            {
                writer.WritePropertyName(claim.Key);
                writer.WriteValue(claim.Value);
            }

            writer.WriteEndObject();
        }

        internal bool IsPrimitiveToken(JsonToken token)
        {
            switch (token)
            {
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.String:
                case JsonToken.Boolean:
                case JsonToken.Undefined:
                case JsonToken.Null:
                case JsonToken.Date:
                case JsonToken.Bytes:
                    return true;
                default:
                    return false;
            }
        }
    }

    [JsonConverter(typeof(IdentityResponseJsonConverter))]
    public class IdentityResponse : Collection<KeyValuePair<string, object>>
    {
        /// <summary>Initializes a new instance of the <see cref="IdentityResponse" /> class.</summary>
        /// <param name="claims">A variable-length parameters list containing claims.</param>
        public IdentityResponse(IEnumerable<KeyValuePair<string, string>> claims)
        {
            foreach (var claim in claims)
            {
                this.Add(new KeyValuePair<string, object>(claim.Key, claim.Value));
            }
        }

        /// <summary>Initializes a new instance of the <see cref="IdentityResponse" /> class.</summary>
        /// <param name="claims">A variable-length parameters list containing claims.</param>
        public IdentityResponse(params Claim[] claims)
        {
            foreach (var claim in claims)
            {
                this.Add(new KeyValuePair<string, object>(claim.Type, claim.Value));
            }
        }

        /// <summary>Gets the identifier.</summary>
        /// <value>The identifier.</value>
        public string Id
        {
            get
            {
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.Id || x.Key == Constants.ClaimType.Id);

                return claim.Value.ToString();
            }
        }

        /// <summary>Gets the issuer.</summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get
            {
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.Issuer || x.Key == Constants.ClaimType.Issuer);

                return claim.Value.ToString();
            }
        }

        /// <summary>Gets the subject.</summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get
            {
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.Subject || x.Key == Constants.ClaimType.Name);

                return claim.Value.ToString();
            }
        }

        /// <summary>Gets the audience.</summary>
        /// <value>The audience.</value>
        public string Audience
        {
            get
            {
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.Audience || x.Key == Constants.ClaimType.RedirectUri);

                return claim.Value.ToString();
            }
        }

        /// <summary>Gets the scope.</summary>
        /// <value>The scope.</value>
        public IEnumerable<string> Scope
        {
            get
            {
                var scopes = this.Where(x => x.Key == Constants.ClaimType.Scope);

                if (scopes.Any())
                {
                    return scopes.Select(x => x.Value.ToString());
                }

                return Enumerable.Empty<string>();
            }
        }

        /// <summary>Gets the expiration time.</summary>
        /// <value>The expiration time.</value>
        public DateTimeOffset ExpirationTime
        {
            get
            {
                DateTimeOffset dt;
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.ExpirationTime || x.Key == Constants.ClaimType.Expiration);

                if (DateTimeOffset.TryParse(claim.Value.ToString(), out dt))
                {
                    return dt;
                }

                return DateTimeOffset.MinValue;
            }
        }

        /// <summary>Gets the valid from time.</summary>
        /// <value>The valid from time.</value>
        public DateTimeOffset ValidFrom
        {
            get
            {
                DateTimeOffset dt;
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.NotBefore || x.Key == Constants.ClaimType.ValidFrom);

                if (DateTimeOffset.TryParse(claim.Value.ToString(), out dt))
                {
                    return dt;
                }

                return DateTimeOffset.MinValue;
            }
        }

        /// <summary>Gets the created time.</summary>
        /// <value>The created time.</value>
        public DateTimeOffset Created
        {
            get
            {
                DateTimeOffset dt;
                var claim = this.FirstOrDefault(x => x.Key == Constants.JwtClaimType.IssuedAt || x.Key == Constants.ClaimType.AuthenticationInstant);

                if (DateTimeOffset.TryParse(claim.Value.ToString(), out dt))
                {
                    return dt;
                }

                return DateTimeOffset.MinValue;
            }
        }
    }
}
