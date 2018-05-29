using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LazyWelfare.ServerCore
{
    /// <summary>
    /// 操作返回结果对象
    /// </summary>
    public class TaskResult
    {

        #region 构造函数...

        /// <summary>
        /// 默认无参数的构造函数
        /// </summary>
        public TaskResult() { Succeeded = true; }

        /// <summary>
        /// 预定义操作状态的构造函数
        /// </summary>
        /// <param name="success">预定义操作执行状态</param>
        public TaskResult(bool success)
        {
            Succeeded = success;
        }

        /// <summary>
        /// 以[操作返回结果对象]定义的构造函数，从 status 中提取错误信息，无则 success=ture
        /// </summary>
        /// <param name="status">操作返回结果对象</param>
        public TaskResult(TaskResult status)
        {
            AddErrors(status);
        }

        /// <summary>
        /// 以预定义错误信息的构造函数
        /// </summary>
        /// <param name="errors">错误信息</param>
        public TaskResult(IEnumerable<string> errors)
        {
            if (errors == null || errors.Count() == 0)
            {
                Succeeded = true;
            }
            else
            {
                AddErrors(errors);
            }
        }

        /// <summary>
        /// 以预定义错误信息的构造函数
        /// </summary>
        /// <param name="errors">错误信息</param>
        public TaskResult(params string[] errors)
        {
            if (errors == null)
            {
                Succeeded = true;
            }
            else
            {
                Succeeded = !(errors.Count() > 0);
                Errors = errors.ToList();
            }
        }

        #endregion

        #region 属性...

        /// <summary>
        /// 操作执行状态结果
        /// </summary>
        public bool Succeeded { get; private set; }

        /// <summary>
        /// 操作执行错误记录
        /// </summary>
        public List<string> Errors { get; private set; }

        #endregion

        #region 方法...

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="error">单条错误</param>
        /// <returns>对象本身</returns>
        public TaskResult AddError(string error)
        {
            if (Errors == null)
                Errors = new List<string>();
            Errors.Add(error);
            Succeeded = false;
            return this;
        }

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="errors">多条错误</param>
        /// <returns>对象本身</returns>
        public TaskResult AddErrors(IEnumerable<string> errors)
        {
            if (Errors == null)
                Errors = new List<string>();
            Errors.AddRange(errors);
            Succeeded = false;
            return this;
        }

        /// <summary>
        /// 添加错误
        /// </summary>
        /// <param name="errors">多条错误</param>
        /// <returns>对象本身</returns>
        public TaskResult AddErrors(params string[] errors)
        {
            return AddErrors(errors);
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <typeparam name="E">中间结果类型</typeparam>
        /// <param name="FindResults">获取执行的中间结果</param>
        /// <param name="FormatError">格式化中间结果返回错误信息</param>
        /// <returns>对象本身</returns>
        public TaskResult AddErrors<E>(Func<IEnumerable<E>> FindResults, Func<E, string> FormatError)
        {
            if (Errors == null) Errors = new List<string>();
            var colloction = FindResults();
            return AddErrors(colloction, FormatError);
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <typeparam name="E">包含错误信息的对象类型</typeparam>
        /// <param name="Colloction">包含错误信息的集合对象</param>
        /// <param name="FormatError">返回提取并格式化后的错误信息</param>
        /// <returns>对象本身</returns>
        public TaskResult AddErrors<E>(IEnumerable<E> Colloction, Func<E, string> FormatError)
        {
            if (Errors == null) Errors = new List<string>();
            foreach (var one in Colloction)
            {
                var error = FormatError(one);
                Errors.Add(error);
            }
            Succeeded = false;
            return this;
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <typeparam name="C">包含错误信息的对象对应的集合类型,如 C 继承 List<E> 的形式</typeparam>
        /// <typeparam name="E">包含错误信息的对象类型</typeparam>
        /// <param name="Colloction">包含错误信息的集合对象</param>
        /// <param name="FormatError">返回提取并格式化后的错误信息</param>
        /// <returns>对象本身</returns>
        public TaskResult AddErrors<C, E>(C Colloction, Func<E, string> FormatError)
            where C : CollectionBase
            where E : class
        {
            if (Errors == null) Errors = new List<string>();
            foreach (var one in Colloction)
            {
                var error = FormatError(one as E);
                Errors.Add(error);
            }
            Succeeded = false;
            return this;
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <param name="status">从[操作返回结果对象]中提取错误信息</param>
        /// <returns>对象本身</returns>
        public TaskResult AddErrors(TaskResult status)
        {
            if (status.Succeeded) return this;
            if (status.Errors == null) return this;
            if (status.Errors.Count == 0) return this;
            AddErrors(status.Errors);
            return this;
        }
        #endregion

    }

    /// <summary>
    /// 操作返回结果对象
    /// </summary>
    /// <typeparam name="T">返回操作数据的类型</typeparam>
    public class TaskResult<T> : TaskResult
    {

        #region 构造函数...

        /// <summary>
        /// 以[操作返回结果对象]定义的构造函数，从 source 中提取错误信息及数据对象，无则 success=ture
        /// </summary>
        /// <param name="source">操作返回结果对象</param>
        public TaskResult(TaskResult<T> source) : base(true)
        {
            Copy(source);
        }

        /// <summary>
        /// 以[操作返回结果对象]定义的构造函数，从 status 中提取错误信息，无则 success=ture
        /// </summary>
        /// <param name="status">操作返回结果对象</param>
        public TaskResult(TaskResult status)
        {
            AddErrors(status);
        }

        /// <summary>
        /// 以预定义操作状态,数据结果,错误信息的构造函数
        /// </summary>
        /// <param name="success">预定义操作状态</param>
        /// <param name="content">预定义数据结果</param>
        /// <param name="errors">预定义错误信息</param>
        public TaskResult(bool success, T content, IEnumerable<string> errors) : base(errors)
        {
            Content = content;
        }

        /// <summary>
        /// 以预定义操作状态,数据结果的构造函数
        /// </summary>
        /// <param name="success">预定义操作状态</param>
        /// <param name="content">预定义数据结果</param>
        public TaskResult(bool success, T content) : base(success)
        {
            Content = content;
        }

        /// <summary>
        ///  以预定义数据结果的构造函数
        /// </summary>
        /// <param name="content">预定义数据结果</param>
        public TaskResult(T content) : base(true)
        {
            Content = content;
        }

        /// <summary>
        /// 以预定义操作状态的构造函数
        /// </summary>
        /// <param name="success">预定义操作状态</param>
        public TaskResult(bool success) : base(success) { }

        /// <summary>
        /// 以预定义错误信息的构造函数
        /// </summary>
        /// <param name="errors">预定义错误信息</param>
        public TaskResult(IEnumerable<string> errors) : base(errors) { }

        /// <summary>
        /// 以预定义错误信息的构造函数
        /// </summary>
        /// <param name="errors">预定义错误信息</param>
        public TaskResult(params string[] errors) : base(errors) { }

        /// <summary>
        /// 默认无参数的构造函数
        /// </summary>
        public TaskResult() : base() { }
        #endregion

        #region 属性...

        /// <summary>
        /// 数据结果
        /// </summary>
        public T Content { get; protected set; }

        #endregion

        #region 方法...

        /// <summary>
        /// 设置数据结果
        /// </summary>
        /// <param name="content">数据结果</param>
        /// <returns>对象本身</returns>
        public TaskResult<T> AddContent(T content)
        {
            Content = content;
            return this;
        }

        /// <summary>
        /// 设置数据结果
        /// </summary>
        /// <param name="content">数据结果</param>
        /// <param name="status">操作返回结果对象,从 source 中提取错误信息，无则 success=ture</param>
        /// <returns>对象本身</returns>
        public TaskResult<T> AddContent(T content, TaskResult status)
        {
            Content = content;
            base.AddErrors(status);
            return this;
        }

        /// <summary>
        /// 对象复制
        /// </summary>
        /// <param name="source">操作返回结果对象,从 source 中提取错误信息及数据对象，无则 success=ture</param>
        /// <returns>对象本身</returns>
        public TaskResult<T> Copy(TaskResult<T> source)
        {
            this.AddContent(source.Content);
            this.AddErrors(source);
            return this;
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="error">错误信息</param>
        /// <returns>对象本身</returns>
        public new TaskResult<T> AddError(string error)
        {
            base.AddError(error);
            return this;
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="errors">错误信息</param>
        /// <returns>对象本身</returns>
        public new TaskResult<T> AddErrors(IEnumerable<string> errors)
        {
            base.AddErrors(errors);
            return this;
        }

        /// <summary>
        /// 添加错误信息
        /// </summary>
        /// <param name="status">操作返回结果对象,从 source 中提取错误信息，无则 success=ture</param>
        /// <returns>对象本身</returns>
        public new TaskResult<T> AddErrors(TaskResult status)
        {
            base.AddErrors(status);
            return this;
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <typeparam name="E">中间结果类型</typeparam>
        /// <param name="FindResults">获取执行的中间结果</param>
        /// <param name="FormatError">格式化中间结果返回错误信息</param>
        /// <returns>对象本身</returns>
        public new TaskResult<T> AddErrors<E>(Func<IEnumerable<E>> FindResults, Func<E, string> FormatError)
        {
            base.AddErrors(FindResults, FormatError);
            return this;
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <typeparam name="E">包含错误信息的对象类型</typeparam>
        /// <param name="Colloction">包含错误信息的集合对象</param>
        /// <param name="FormatError">返回提取并格式化后的错误信息</param>
        /// <returns>对象本身</returns>
        public new TaskResult<T> AddErrors<E>(IEnumerable<E> Colloction, Func<E, string> FormatError)
        {
            base.AddErrors(Colloction, FormatError);
            return this;
        }

        /// <summary>
        /// 添加错误(自定义错误格式)
        /// </summary>
        /// <typeparam name="C">包含错误信息的对象对应的集合类型,如 C 继承 List<E> 的形式</typeparam>
        /// <typeparam name="E">包含错误信息的对象类型</typeparam>
        /// <param name="Colloction">包含错误信息的集合对象</param>
        /// <param name="FormatError">返回提取并格式化后的错误信息</param>
        /// <returns>对象本身</returns>
        public new TaskResult<T> AddErrors<C, E>(C Colloction, Func<E, string> FormatError)
            where C : CollectionBase
            where E : class
        {
            base.AddErrors(Colloction, FormatError);
            return this;
        }

        #endregion

    }
}
