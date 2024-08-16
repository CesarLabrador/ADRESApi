using AdresApi.Models;
using AdresApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AdresApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RRAController : ControllerBase
    {
        private readonly ILogger<RRAController> _logger;
        private readonly IDatab S_DataBase ;

        public RRAController(ILogger<RRAController> logger,IDatab database)
        {
            _logger = logger;
            S_DataBase= database;
        }

        [HttpGet]
        public void Get()
        {
            Console.Write("ADRES RRA");
        }

        [HttpPatch]
        [Route("v1")]
        [AllowAnonymous]
        public DefaultResponse Update(RRRAData Data)
        {
            var _Res = new DefaultResponse();
            try
            {
                _Res = S_DataBase.UpdateRecord(Data);
            }
            catch (Exception ex)
            {
                _Res.isValid = false; _Res.stringValue = ex.Message;
            }
            return _Res;
        }

        [HttpDelete]
        [Route("v1/{id}")]
        [AllowAnonymous]
        public DefaultResponse Delete(double id)
        {
            var _Res = new DefaultResponse();
            try
            {
                _Res = S_DataBase.DeleteRecord(id);
            }
            catch (Exception ex)
            {
                _Res.isValid = false; _Res.stringValue = ex.Message;
            }
            return _Res;
        }

        [HttpPost]
        [Route("v1")]
        [AllowAnonymous]
        public DefaultResponse Add(RRAData Data)
        {
            var _Res = new DefaultResponse();
            try
            {
                _Res= S_DataBase.AddRecord(Data);
            }
            catch (Exception ex)
            {
                _Res.isValid = false; _Res.stringValue = ex.Message;
            }
            return _Res;
        }

        [HttpPost]
        [Route("v1/filter")]
        [AllowAnonymous]
        public RFilterData Filter(FilterData Data)
        {
            var _Res = new RFilterData();
            try
            {
                _Res = S_DataBase.FilterData(Data);
            }
            catch (Exception ex)
            {
                _Res.isValid = false; _Res.stringValue = ex.Message;
            }
            return _Res;
        }

        [HttpGet]
        [Route("v1/{id}")]
        [AllowAnonymous]
        public RData GetData(double id)
        {
            var _Res = new RData();
            try
            {
                _Res = S_DataBase.GetData(id);
            }
            catch (Exception ex)
            {
                _Res.isValid = false; _Res.stringValue = ex.Message;
            }
            return _Res;
        }

    }
}
