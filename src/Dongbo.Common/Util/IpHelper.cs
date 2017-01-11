﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Web;

namespace Dongbo.Common.Util
{
    [System.Serializable]
    public class IpHelper
    {

        #region ==================== Private Filed ====================

        private static string _LocalServerIP;
        private static object _lockPad = new object();

        #endregion



        #region ==================== Constructed Method ====================

        public IpHelper()
        {
        }

        #endregion



        #region ==================== Public Method ====================

        /// <summary>
        /// 得到客户端IP
        /// </summary>
        /// <param name="httpContext">HttpContext对象</param>
        /// <returns>客户端ip字符串</returns>
        public static string GetClientIp(HttpContext httpContext)
        {
            string result = string.Empty;

            try
            {
                result = httpContext.Request.ServerVariables["REMOTE_ADDR"];
            }
            catch
            {
                return "";
            }

            return result;
        }

        /// <summary>
        /// 得到客户端SessionId
        /// </summary>
        /// <param name="httpContext">HttpContext对象</param>
        /// <returns>客户端的SessionId</returns>
        public static string GetClientSessionId(HttpContext httpContext)
        {
            string result = String.Empty;
            if (httpContext.Session != null
                && !String.IsNullOrEmpty(httpContext.Session.SessionID))
            {
                result = httpContext.Session.SessionID;
            }

            return result;
        }

        /// <summary>
        /// 获得服务器IP
        /// </summary>
        /// <returns>服务器IP</returns>
        [Obsolete("请不要使用该方法，使用GetServerIpV4IP或者GetServerIpV6IP来获取Ip地址。")]
        public static string GetServerIp()
        {
            if (_LocalServerIP == null)
            {
                string result = String.Empty;
                IPAddress[] addressList = Dns.GetHostEntry(Dns.GetHostName()).AddressList; //获得 IP 列表
                foreach (IPAddress ip in addressList)
                {
                    if ((ip.AddressFamily & AddressFamily.InterNetworkV6) == AddressFamily.InterNetworkV6)
                    {
                        continue;
                    }
                    else
                    {
                        result = ip.ToString();
                    }
                }

                if (result != string.Empty)
                {
                    lock (_lockPad)
                    {
                        _LocalServerIP = result;
                    }
                }
                else
                {
                    throw new Exception("Can not get server ipv4 address!");
                }
            }
            return _LocalServerIP;
        }


        /// <summary>
        /// 通过IP类型获取Ip
        /// </summary>
        /// <param name="ipType"></param>
        /// <returns></returns>
        private static string GetIPByType(AddressFamily ipType)
        {
            try
            {
                string result = String.Empty;
                NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
                foreach (NetworkInterface ni in interfaces)
                {
                    bool find = false;
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet && ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == ipType)
                            {
                                result = ip.Address.ToString();
                                find = true;
                                break;
                            }
                        }
                    }
                    if (find) break;
                }
                return result;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取IpV4IP地址（此方法尽量不要使用，影响性能）
        /// </summary>
        /// <returns></returns>
        public static string GetServerV4IP()
        {
            return GetIPByType(AddressFamily.InterNetwork);
        }

        /// <summary>
        /// 获取服务器HOST名称
        /// </summary>
        /// <returns></returns>
        public static string GetServerHostName()
        {
            try
            {
                return System.Net.Dns.GetHostName();
            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取IpV6IP地址
        /// </summary>
        /// <returns></returns>
        public static string GetServerV6IP()
        {
            return GetIPByType(AddressFamily.InterNetworkV6);
        }


        /// <summary>
        /// 将127.0.0.1形式的IP地址转换成10进制整数形式IP地址
        /// </summary>
        /// <param name="Ip">127.0.0.1形式的IP</param>
        /// <returns>10进制整数形式的IP</returns>
        public static long IpToLong(string Ip)
        {
            if (Ip == null) return 0;

            string[] IpArray = Ip.Split('.');

            if (IpArray.Length != 4) return 0;

            long[] ip = new long[4];

            for (int i = 0; i < 4; i++)
                ip[i] = SafeConvert.ToInt64(IpArray[i], 0);

            return (ip[0] << 24) + (ip[1] << 16) + (ip[2] << 8) + ip[3];
        }

        /// <summary>
        /// 将10进制整数形式转换成127.0.0.1形式的IP地址
        /// </summary>
        /// <param name="longIP">10进制整数形式的IP</param>
        /// <returns>127.0.0.1形式的IP</returns>
        public static String LongToIP(long longIP)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(longIP >> 24);
            sb.Append(".");
            sb.Append((longIP & 0x00FFFFFF) >> 16);
            sb.Append(".");
            sb.Append((longIP & 0x0000FFFF) >> 8);
            sb.Append(".");
            sb.Append(longIP & 0x000000FF);
            return sb.ToString();
        }

        #endregion

    }
}
