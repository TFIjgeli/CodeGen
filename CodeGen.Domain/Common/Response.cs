using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CodeGen.Domain.Common
{
    public class Response
    {
        public static Response<T> Fail<T>(T data, string message) =>
            new Response<T>(data, message, true);

        public static Response<T> Success<T>(T data) =>
            new Response<T>(data, string.Empty, false);
    }

    public record Response<T> : CQRSResponse
    {
        public Response()
        {
        }

        public Response(T data, string message, bool error)
        {
            this.Data = data;
            this.Message = message;
            this.Error = error;
        }

        public T Data { get; set; }

    }



    public record CQRSResponse
    {
        public HttpStatusCode StatusCode { get; init; } = HttpStatusCode.OK;
        public string Message { get; init; }
        public bool Error { get; set; }

        public ModelStateError ModelStateError
        {
            get
            {
                if (Error)
                    return new ModelStateError
                    {
                        ErrorMessage = Message,
                        Title = "Error"
                    };

                return null;
            }
        }
    }

    public class ModelStateError
    {
        public string Title { get; set; }
        public string ErrorMessage { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
