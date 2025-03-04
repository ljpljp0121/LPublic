
using System;

// 初始化顺序特性
//用于玩家角色初始化组件顺序
[AttributeUsage(AttributeTargets.Class)]
public class InitializeOrderAttribute : Attribute
{
    public int Order { get; }
    public InitializeOrderAttribute(int order) => Order = order;
}