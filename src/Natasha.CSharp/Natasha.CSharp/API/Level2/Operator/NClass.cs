﻿using Natasha.CSharp;
/// <summary>
/// 默认创建一个公有的类
/// </summary>
public class NClass : NHandler<NClass>
    {

        public NClass()
        {

            Link = this;
            this.Class();

        }

    }

