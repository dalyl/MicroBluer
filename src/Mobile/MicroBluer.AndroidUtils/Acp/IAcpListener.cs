namespace MicroBluer.AndroidUtils.Acp
{
    using System;
    using System.Collections.Generic;
    /// <summary>
    /// Created by hupei on 2016/4/26.
    /// </summary>
    public interface IAcpListener
    {
        /// <summary>
        /// 同意
        /// </summary>
        void OnGranted();

        /// <summary>
        /// 拒绝
        /// </summary>
        void OnDenied(LinkedList<string> permissions);
    }

    public class AnonymousAcpListener : IAcpListener
    {
        public Action<LinkedList<string>> Denied { get; }
        public Action Granted { get; }
        public AnonymousAcpListener(Action<LinkedList<string>> denied = null, Action granted = null)
        {
            Denied = denied;
            Granted = granted;
        }

        public void OnDenied(LinkedList<string> permissions)
        {
            Denied?.Invoke(permissions);
        }

        public void OnGranted()
        {
            Granted?.Invoke();
        }
    }
}