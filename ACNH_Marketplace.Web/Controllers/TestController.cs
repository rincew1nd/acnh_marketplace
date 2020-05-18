// <copyright file="TestController.cs" company="Cattleya">
// Copyright (c) Cattleya. All rights reserved.
// </copyright>

namespace ACNH_Marketplace.Web.Controllers
{
    using ACNH_Marketplace.Telegram.Services;
    using Microsoft.AspNetCore.Mvc;

    /// <summary>
    /// Test controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestController"/> class.
        /// </summary>
        public TestController()
        {
        }

        /// <summary>
        /// Check is api working.
        /// </summary>
        /// <returns>Stub.</returns>
        [HttpGet]
        [Route("isworking")]
        public bool IsWorking()
        {
            return true;
        }
    }
}