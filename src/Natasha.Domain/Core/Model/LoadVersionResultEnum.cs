﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Natasha.CSharp.Component.Domain
{
    public enum LoadVersionResultEnum
    {
        /// <summary>
        /// 无动作
        /// </summary>
        NoAction,
        /// <summary>
        /// 使用默认的
        /// </summary>
        UseDefault,
        /// <summary>
        /// 使用客户的
        /// </summary>
        UseCustomer,
        /// <summary>
        /// 传递给版本控制继续处理
        /// </summary>
        PassToNextHandler
    }
}
