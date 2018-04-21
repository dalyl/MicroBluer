using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LazyWelfare.AndroidMobile.Models;

namespace LazyWelfare.AndroidMobile.Views
{
    public  class _Layout
    {

        public static string BeginHtml => "<html>";
        public static string EndHtml => "</html>";
        public static string BeginBody => "<body>";
        public static string EndBody => "</body>";


        public static string RenderHeader()
        {
            var bulider = new StringBuilder();
            bulider.Append(@"<head>
                                <meta charset=""utf-8"" />
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                                <link rel=""stylesheet"" href=""bootstrap/bootstrap.css"" />
                                <link rel=""stylesheet"" href=""bootstrap/bootstrap-theme.css"" />
                                <link rel=""stylesheet"" href=""style.css"" />
                            </head>");

            return bulider.ToString();

        }

        public static string RenderMenuContainer(HomeModel model)
        {
            return $@"<div class=""container"">
                                <div class=""navbar-header navbar-default"">
                                    <button type=""button"" class=""navbar-toggle collapsed"" data-toggle=""collapse"" data-target=""#bs-example-navbar-collapse-1""
                                            aria-expanded=""false"">
                                        <span class=""sr-only"">Toggle navigation</span>
                                        <span class=""icon-bar""></span>
                                        <span class=""icon-bar""></span>
                                        <span class=""icon-bar""></span>
                                    </button>
                                    <a class=""navbar-brand"" href=""#"">{model.Header}</a>
                                </div>
                                <div class=""navbar-collapse collapse"" id=""bs-example-navbar-collapse-1"" aria-expanded=""false"" style=""height: 1px;"">
                                    <ul class=""nav navbar-nav"">
                                        <li class=""active"">
                                            <a href=""#"">
                                                Link
                                                <span class=""sr-only"">(current)</span>
                                            </a>
                                        </li>
                                        <li>
                                            <a href=""#"">Link</a>
                                        </li>
                                        <li class=""dropdown"">
                                            <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-haspopup=""true"" aria-expanded=""false"">
                                                Dropdown
                                                <span class=""caret""></span>
                                            </a>
                                            <ul class=""dropdown-menu"">
                                                <li>
                                                    <a href=""#"">Action</a>
                                                </li>
                                                <li>
                                                    <a href=""#"">Another action</a>
                                                </li>
                                                <li>
                                                    <a href=""#"">Something else here</a>
                                                </li>
                                                <li role=""separator"" class=""divider""></li>
                                                <li>
                                                    <a href=""#"">Separated link</a>
                                                </li>
                                                <li role=""separator"" class=""divider""></li>
                                                <li>
                                                    <a href=""#"">One more separated link</a>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                    <form class=""navbar-form navbar-left"">
                                        <div class=""form-group"">
                                            <input type=""text"" class=""form-control"" placeholder=""Search"">
                                        </div>
                                        <button type=""submit"" class=""btn btn-default"">Submit</button>
                                    </form>
                                    <ul class=""nav navbar-nav navbar-right"">
                                        <li>
                                            <a href=""#"">Link</a>
                                        </li>
                                        <li class=""dropdown"">
                                            <a href=""#"" class=""dropdown-toggle"" data-toggle=""dropdown"" role=""button"" aria-haspopup=""true"" aria-expanded=""false"">
                                                Dropdown
                                                <span class=""caret""></span>
                                            </a>
                                            <ul class=""dropdown-menu"">
                                                <li>
                                                    <a href=""#"">Action</a>
                                                </li>
                                                <li>
                                                    <a href=""#"">Another action</a>
                                                </li>
                                                <li>
                                                    <a href=""#"">Something else here</a>
                                                </li>
                                                <li role=""separator"" class=""divider""></li>
                                                <li>
                                                    <a href=""#"">Separated link</a>
                                                </li>
                                            </ul>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                            <div class=""clearfix""></div>";

        }

        public static string RenderScript()
        {
            return @" <script src=""jquery.min.js""></script>
                              <script src=""bootstrap/bootstrap.min.js""></script>";

        }
    }
}