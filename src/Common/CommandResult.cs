using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dongbo.Common
{
    /// <summary> 公用操作结果类 </summary>
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Msg { get; set; }
        public ErrCode ErrCode { get; set; }

        public static CommandResult Ok()
        {
            return new CommandResult() { Success = true, Msg = "", ErrCode = ErrCode.Success };
        }
        public static CommandResult Faild(string msg, ErrCode errcode)
        {
            return new CommandResult { Msg = msg, Success = false, ErrCode = errcode };
        }
        public static CommandResult Faild(string msg)
        {
            return Faild(msg, ErrCode.Unknown);
        }

    }
    /// <summary> 公共错误码 </summary>
    public enum ErrCode
    {
        /// <summary> 默认值为异常 </summary>
        Error,
        /// <summary> 操作成功 </summary>
        Success,
        /// <summary> 未知错误 </summary>
        Unknown
    }
}
