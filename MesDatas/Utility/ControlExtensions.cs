using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MesDatas.Utility
{
    public static class ControlExtensions
    {
        /// <summary>
        /// [扩展方法] UI线程安全获取属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="control"></param>
        /// <param name="propertyGetter"></param>
        /// <returns></returns>
        public static T GetPropertySafely<T, TControl>(this TControl control, Func<TControl, T> propertyGetter) where TControl : Control
        {
            if (control.InvokeRequired)
            {
                // 使用Invoke在UI线程上执行委托  
                return (T)control.Invoke(new Func<T>(() => propertyGetter(control)));
            }
            else
            {
                // 如果不在UI线程上，则直接返回值  
                return propertyGetter(control);
            }
        }

        /// <summary>
        /// [扩展方法] UI线程安全执行无返回值的方法（如 Update, Clear）
        /// </summary>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="control"></param>
        /// <param name="propertyGetter"></param>
        public static void ExecuteSafely<TControl>(this TControl control, Action<TControl> propertyGetter) where TControl : Control
        {
            if (control.InvokeRequired)
            {
                // 使用Invoke在UI线程上执行委托  
                control.Invoke(new Action(() => propertyGetter(control)));
            }
            else
            {
                // 如果不在UI线程上，则直接返回值  
                propertyGetter(control);
            }
        }

        /// <summary>
        /// [扩展方法] UI线程安全设置属性值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TControl"></typeparam>
        /// <param name="control"></param>
        /// <param name="propertyGetter"></param>
        public static void SetPropertySafely<TControl, T>(this TControl control, T newValue, Action<TControl, T> propertySetter) where TControl : Control
        {
            if (control.InvokeRequired)
            {
                // 直接传递 propertySetter 委托给 Invoke 方法  
                control.Invoke(propertySetter, control, newValue);
            }
            else
            {
                // 如果已经在UI线程上，则直接调用 propertySetter 委托  
                propertySetter(control, newValue);
            }
        }

        public static Task InvokeAsync(this Control control, Action action)
        {
            return Task.Factory.FromAsync(control.BeginInvoke(action), control.EndInvoke);
        }

        public static Task<T> InvokeAsync<T>(this Control control, Func<T> func)
        {
            return Task.Factory.FromAsync(
                control.BeginInvoke(func),
                (asyncResult) => (T)control.EndInvoke(asyncResult));
        }
    }
}
