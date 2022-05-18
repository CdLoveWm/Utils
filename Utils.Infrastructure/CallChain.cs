using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utils.Infrastructure
{
    /// <summary>
    /// 调用链
    /// </summary>
    public class CallChain
    {
        /// <summary>
        /// 获取方法调用链
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetCallChain()
        {
            var stackFrames = new StackTrace(1, true).GetFrames();
            var callchains = stackFrames.Select((sf, i) => {
                var m = sf.GetMethod();
                return $"{m.DeclaringType.FullName}.{m.Name}";
            })
            .Reverse();

            return callchains;
        }

        /// <summary>
        /// 当前执行的类、方法名称
        /// </summary>
        public static void PrintCurrent()
        {
            #region 方式一（可用）
            //// 获取当前执行方法名称
            //var currentMethodName = MethodBase.GetCurrentMethod().Name;
            //Console.WriteLine($"当前执行方法名称：{currentMethodName}");
            //// 获取当前执行方法的类名
            //var currentClassName = MethodBase.GetCurrentMethod().DeclaringType.Name;
            //Console.WriteLine($"当前执行类名称：{currentClassName}");
            #endregion

            #region 方式二（可用）
            var stackFrames = new StackTrace(true).GetFrames();
            // 0是本身，1是调用方，2是调用方的调用方...以此类推
            var currentMethodName = stackFrames[0].GetMethod().Name;
            var currentClassName = stackFrames[0].GetMethod().DeclaringType.Name;
            var preMethodName = stackFrames[1].GetMethod().Name;
            var preClassName = stackFrames[1].GetMethod().DeclaringType.Name;

            Console.WriteLine($"当前执行方法名称：{currentMethodName}，类名称：{currentClassName}");
            Console.WriteLine($"调用方法名称：{preMethodName}，类名称：{preClassName}");

            #endregion
        }
    }

    /// <summary>
    /// 测试获取方法调用链
    /// </summary>
    public class TestCallChain
    {
        public static void Test1() => Test2();
        public static void Test2() => Test3();
        public static void Test3() => Test4();
        public static void Test4()
        {
            var callChains = CallChain.GetCallChain();
            Console.WriteLine(string.Join("-->", callChains.Select(it => it)));
        }
    }
}
