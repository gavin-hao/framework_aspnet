using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Dongbo.Common.Util
{
    public class StringHelper
    {
        /// <summary>
        /// 截断字符串（取前len个字符）
        /// </summary>
        /// <param name="s"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        public static string SubString(string s, int len)
        {
            if (s.Length <= len)
            {
                return s;
            }
            else
            {
                return s.Substring(0, len);
            }
        }

        /// <summary>
        /// 转换数字为中文数字
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public static string Number2CHS(int number)
        {
            return Number2CHS(number.ToString());
        }

        /// <summary>
        /// 把数字字符串转换为中文数字字符串
        /// </summary>
        /// <param name="numberstr"></param>
        /// <returns></returns>
        public static string Number2CHS(string numberstr)
        {
            string[] nums = new string[]{
                "零","一","二","三","四","五","六","七","八","九"
            };
            int zero = '0';
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < numberstr.Length; i++)
            {
                if (Char.IsDigit(numberstr[i]))
                {
                    result.Append(nums[numberstr[i] - zero]);
                }
                else
                {
                    throw new ArithmeticException("输入的数字字符串中含有非数字字符：" + numberstr[i].ToString());
                }
            }
            return result.ToString();
        }

        /// <summary>
        /// 把中文数字字符串转换为数字字符串
        /// </summary>
        /// <param name="chsString"></param>
        /// <returns></returns>
        public static string CHS2NumberString(string chsString)
        {
            string numstr = "零一二三四五六七八九";
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < chsString.Length; i++)
            {
                int pos = numstr.IndexOf(chsString[i]);
                if (pos == -1)
                {
                    result.Append(chsString[i]);
                }
                else
                {
                    result.Append(pos);
                }
            }
            return result.ToString();
        }


        /// <summary>
        /// 剔除script脚本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterScript(string content)
        {
            string regexstr = @"<script[^>]*>([\s\S](?!<script))*?</script>";
            return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 剔除style脚本
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string FilterStyle(string content)
        {
            string regexstr = @"<style[^>]*>([\s\S](?!<style))*?</style>";
            return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 剔除HTML标签
        /// </summary>
        /// <param name="HTMLStr"></param>
        /// <returns></returns>
        public static string RemoveHtmlTags(string HTMLStr)
        {
            return System.Text.RegularExpressions.Regex.Replace(HTMLStr, "<[^>]*>", "");
        }

        /// <summary>
        /// byte数组转为16进制字符串
        /// </summary>
        /// <param name="byteArray"></param>
        /// <returns></returns>
        public static string ByteToHex(byte[] byteArray)
        {
            string outString = "";

            foreach (Byte b in byteArray)
                outString += b.ToString("X2");
            return outString;
        }


        /// <summary>
        /// 16进制字符串转为byte数组
        /// </summary>
        /// <param name="hexString"></param>
        /// <returns></returns>
        public static byte[] HexToByte(string hexString)
        {
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static string FilterSpecial(string str)
        {
            if (string.IsNullOrEmpty(str)) //如果字符串为空，直接返回。
            {
                return str;
            }
            else
            {
                str = str.Replace("'", "‘");
                //str = str.Replace("<", "");
                //str = str.Replace(">", "");
                str = str.Replace("%", "％");
                //str = str.Replace("'delete", "");
                str = str.Replace("''", "‘");
                str = str.Replace("\"\"", "");
                str = str.Replace(",", "，");
                //str = str.Replace(".", "。");
                str = str.Replace(">=", "");
                str = str.Replace("=<", "");
                str = str.Replace(";", "：");
                str = str.Replace("||", "");
                str = str.Replace("[", "");
                str = str.Replace("]", "");
                //str = str.Replace("&", "");
                str = str.Replace("/", "");
                str = str.Replace("|", "");
                str = str.Replace("?", "？");
                //str = str.Replace(" ", "");
                return str;
            }
        }

        /// <summary>
        /// 在字符串数组中查找指定值是否存在
        /// </summary>
        /// <param name="arStr">数组</param>
        /// <param name="strFind">值</param>
        /// <returns></returns>
        public static bool SearchValueInArrayIsExist(string[] arStr, string strFind)
        {
            bool IsExist = false;
            for (int i = 0; i < arStr.Length; i++)
            {

                if (arStr[i] == strFind)
                {
                    IsExist = true;
                    break;
                }
            }
            return IsExist;
        }

        public static string ClearHtml(string html)
        {
            return Regex.Replace(html, @"<[^>]*>", string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 检测一个字符串是否可以转化为日期。
        /// </summary>
        /// <param name="date">日期字符串。</param>
        /// <returns>是否可以转换。</returns>
        public static bool IsStringDate(string date)
        {
            DateTime dt;
            try
            {
                dt = DateTime.Parse(date);
            }
            catch (FormatException e)
            {
                //日期格式不正确时
                e.ToString();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取拆分符左边的字符串
        /// </summary>
        /// <param name="String">需要做处理的字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>按照拆分字符拆分好的左侧字符串</returns>
        public static string GetLeftSplitString(string String, char splitChar)
        {
            string result = null;
            string[] tempString = String.Split(splitChar);
            if (tempString.Length > 0)
            {
                result = tempString[0].ToString();
            }
            return result;
        }

        /// <summary>
        /// 获取拆分符右边的字符串
        /// </summary>
        /// <param name="String">需要做处理的字符串</param>
        /// <param name="splitChar">拆分字符</param>
        /// <returns>按照拆分字符拆分号的右侧字符串</returns>
        public static string GetRightSplitString(string String, char splitChar)
        {
            string result = null;
            string[] tempString = String.Split(splitChar);
            if (tempString.Length > 0)
            {
                result = tempString[tempString.Length - 1].ToString();
            }
            return result;
        }

        /// <summary>
        /// 是否数字字符串
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumber(string inputData)
        {
            Regex RegNumber = new Regex("^[0-9]+$");//正整数
            Match m = RegNumber.Match(inputData);
            return m.Success;
        }

        /// <summary>
        /// 是否数字字符串 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsNumberSign(string inputData)
        {
            Regex RegNumberSign = new Regex("^[+-]?[0-9]+$");//正负整数
            Match m = RegNumberSign.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimal(string inputData)
        {
            Regex RegDecimal = new Regex("^[0-9]+[.]?[0-9]+$");//小数
            Match m = RegDecimal.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 是否是浮点数 可带正负号
        /// </summary>
        /// <param name="inputData">输入字符串</param>
        /// <returns></returns>
        public static bool IsDecimalSign(string inputData)
        {
            Regex RegDecimalSign = new Regex("^[+-]?[0-9]+[.]?[0-9]+$"); //等价于^[+-]?\d+[.]?\d+$
            Match m = RegDecimalSign.Match(inputData);
            return m.Success;
        }

        /// <summary>
        /// 检测某字符是否为英文字母
        /// </summary>
        /// <param name="str">要检查的字符串</param>
        /// <returns>True表示是英文字母,False表示不是英文字母</returns>
        public static bool IsEnglish(string str)
        {

            if (string.IsNullOrEmpty(str))
            {

                return false;

            }
            else
            {

                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(@"^[a-zA-Z_]+$");//正则表达式 验证英文、数字、下划线和点Regex(@"^[0-9a-zA-Z_]+$");--^[a-zA-Z0-9_\u4e00-\u9fa5]+$ 

                return reg.IsMatch(str);
            }


        }

        /// <summary>
        /// 检测是否有中文字符
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsChinese(string inputData)
        {
            Regex RegChinese = new Regex("[\u4e00-\u9fa5]+");//中文
            Match m = RegChinese.Match(inputData);
            return m.Success;
        }

        #region 空行
        /// <summary>
        /// 是否有空行
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsTrimRow(string inputData)
        {
            Regex RegTrimRow = new Regex(@"\n[\s| ]*\r");//空行
            Match m = RegTrimRow.Match(inputData);
            return m.Success;
        }
        #endregion


        #region 邮件地址
        /// <summary>
        /// 判断合法的E-Mail
        /// </summary>
        /// <param name="email">要检查的字符串</param>
        /// <returns>True表示是合法Email,False表示不是合法Email</returns>
        public static bool IsEmail(string email)
        {
            Regex EmailRegex = new Regex(
            @"^([a-zA-Z0-9][_\.\-]*)+\@([A-Za-z0-9])+((\.|-|_)[A-Za-z0-9]+)*((\.[A-Za-z0-9]{2,4}){1,2})$",
            RegexOptions.Compiled);
            if (string.IsNullOrEmpty(email))
                return false;

            email = email.Trim();
            return EmailRegex.IsMatch(email);
        }


        #endregion


        #region 手机
        /// <summary>
        /// 是否是国内手机
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsMobile(string inputData)
        {
            Regex RegMobile = new Regex(@"^((\(\d{2,3}\))|(\d{3}\-))?1[3|5|4|8]\d{9}$", RegexOptions.Compiled);//国内手机
            Match m = RegMobile.Match(inputData);
            return m.Success;
        }


        #endregion
        #region URL网址
        /// <summary>
        /// 是否是带http://的网址
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsUrl(string inputData)
        {
            Regex RegUrl = new Regex("^http://([w-]+.)+[w-]+(/[w-./ %&=]*)$");//带http://的网址
            Match m = RegUrl.Match(inputData);
            return m.Success;
        }
        #endregion

        /// <summary>
        ///  判断是否有非法字符
        /// </summary>
        /// <param name="strString"></param>
        /// <returns>返回TRUE表示有非法字符，返回FALSE表示没有非法字符。</returns>
        public static bool IsInvalidStr(string strString)
        {
            bool outValue = false;
            if (strString != null && strString.Length > 0)
            {
                string[] bidStrlist = new string[21];
                bidStrlist[0] = "'";
                bidStrlist[1] = ";";
                bidStrlist[2] = ":";
                bidStrlist[3] = "%";
                bidStrlist[4] = "@";
                bidStrlist[5] = "&";
                bidStrlist[6] = "#";
                bidStrlist[7] = "\"";
                bidStrlist[8] = "net user";
                bidStrlist[9] = "exec";
                bidStrlist[10] = "net localgroup";
                bidStrlist[11] = "select";
                bidStrlist[12] = "asc";
                bidStrlist[13] = "char";
                bidStrlist[14] = "mid";
                bidStrlist[15] = "insert";
                bidStrlist[16] = "delete";
                bidStrlist[17] = "drop";
                bidStrlist[18] = "truncate";
                bidStrlist[19] = "xp_cmdshell";
                bidStrlist[19] = "order";




                string tempStr = strString.ToLower();
                for (int i = 0; i < bidStrlist.Length; i++)
                {
                    //if (tempStr.IndexOf(bidStrlist[i]) != -1)
                    if (tempStr == bidStrlist[i])
                    {
                        outValue = true;
                        break;
                    }
                }
            }
            return outValue;
        }



        /// <summary>
        /// 转换特殊字符为全角,防止SQL注入攻击
        /// </summary>
        /// <param name="str">要过滤的字符</param>
        /// <returns>返回全角转换后的字符</returns>
        public static string ChangeFullAngle(string str)
        {
            string tempStr = str;
            if (string.IsNullOrEmpty(tempStr)) //如果字符串为空，直接返回。
            {
                return tempStr;
            }
            else
            {
                tempStr = str.ToLower();
                tempStr = str.Replace("'", "‘");
                tempStr = str.Replace("--", "－－");
                tempStr = str.Replace(";", "；");
                tempStr = str.Replace("exec", "ＥＸＥＣ");
                tempStr = str.Replace("execute", "ＥＸＥＣＵＴＥ");
                tempStr = str.Replace("declare", "ＤＥＣＬＡＲＥ");
                tempStr = str.Replace("update", "ＵＰＤＡＴＥ");
                tempStr = str.Replace("delete", "ＤＥＬＥＴＥ");
                tempStr = str.Replace("insert", "ＩＮＳＥＲＴ");
                tempStr = str.Replace("select", "ＳＥＬＥＣＴ");
                tempStr = str.Replace("<", "＜");
                tempStr = str.Replace(">", "＞");
                tempStr = str.Replace("%", "％");
                tempStr = str.Replace(@"\", "＼");
                tempStr = str.Replace(",", "，");
                tempStr = str.Replace(".", "．");
                tempStr = str.Replace("=", "＝＝");
                tempStr = str.Replace("||", "｜｜");
                tempStr = str.Replace("[", "【");
                tempStr = str.Replace("]", "】");
                tempStr = str.Replace("&", "＆");
                tempStr = str.Replace("/", "／");
                tempStr = str.Replace("|", "｜");
                tempStr = str.Replace("?", "？");
                tempStr = str.Replace("_", "＿");

                return str;
            }
        }
        /// <summary>
        /// 是否是国内身份证号
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public static bool IsIDCard(string inputData)
        {
            Regex RegIDCard = new Regex(@"(^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[A-Z])$)|(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$)");//匹配国内身份证号〕
            Match m = RegIDCard.Match(inputData);
            return m.Success;
        }
        /// <summary>
        /// 按字节数获取字符串的长度
        /// </summary>
        /// <param name="String">要计算的字符串</param>
        /// <returns>字符串的字节数</returns>
        public static int GetByteCount(string String)
        {
            int intCharCount = 0;
            for (int i = 0; i < String.Length; i++)
            {
                if (System.Text.UTF8Encoding.UTF8.GetByteCount(String.Substring(i, 1)) == 3)
                {
                    intCharCount = intCharCount + 2;
                }
                else
                {
                    intCharCount = intCharCount + 1;
                }
            }
            return intCharCount;
        }

        /// <summary>
        /// 截取指定字节数的字符串
        /// </summary>
        /// <param name="Str">原字符串</param>
        /// <param name="Num">要截取的字节数</param>
        /// <returns>截取后的字符串</returns>
        public static string CutStr(string Str, int Num)
        {
            if (Encoding.Default.GetBytes(Str).Length <= Num)
            {
                return Str;
            }
            else
            {
                int CutBytes = 0;
                for (int i = 0; i < Str.Length; i++)
                {
                    if (Convert.ToInt32(Str.ToCharArray().GetValue(i)) > 255)
                    {
                        CutBytes = CutBytes + 2;
                    }
                    else
                    {
                        CutBytes = CutBytes + 1;
                    }
                    if (CutBytes == Num)
                    {
                        return Str.Substring(0, i + 1);
                    }
                    if (CutBytes == Num + 1)
                    {
                        return Str.Substring(0, i);
                    }
                }
                return Str;
            }
        }

        /// <summary>
        /// 防止sql注入
        /// </summary>
        /// <param name="inputName"></param>
        /// <returns></returns>
        public static string SqlReplace(string inputName)
        {
            if (string.IsNullOrEmpty(inputName))
            {
                return string.Empty;
            }

            string[] strCheck = { "'", "%", "--", ";", "EXE", "EXECUTE", "DECLARE", "UPDATE", "DELETE", "INSERT", "SELECT", "_" };
            string[] strReplace = { "＇", "％", "－－", "；", "ＥＸＥＣ", "ＥＸＥＣＵＴＥ", "ＤＥＣＬＡＲＥ", "ＵＰＤＡＴＥ", "ＤＥＬＥＴＥ", "ＩＮＳＥＲＴ", "ＳＥＬＥＣＴ", "＿" };
            for (int i = 0; i < strCheck.Length; i++)
            {
                inputName = Regex.Replace(inputName, strCheck[i], strReplace[i], RegexOptions.IgnoreCase);
            }
            return inputName;
        }

    }
}
