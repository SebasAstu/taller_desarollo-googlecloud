﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NinosConValorAPI;
using NinosConValorAPI.Controllers;
using NinosConValorAPI.Data.Entity;
using NinosConValorAPI.Data.Repository;
using NinosConValorAPI.Models;
using NinosConValorAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTests.NCVRepositoryUT;

namespace UnitTests.ControllersUT
{
    public class KidsControllerUT
    {

        [Fact]
        public async Task GetKidAsync_SingleKid_ReturnStatus()
        {
            var kidModel = new KidModel()
            {
                Id = 1,
                FirstName = "Manuel",
                LastName = "Flores",
                CI = "1234567",
                BirthDate = new DateTime(2010, 9, 12),
                ProgramHouse = "SDA",
                BirthPlace = "Cochabamba",
                Gender = "masculino"
            };
            int kidId = 1;
            var kidServiceMock = new Mock<IKidService>();
            kidServiceMock.Setup(r => r.GetKidAsync(kidId)).ReturnsAsync(kidModel);

            var kidController = new KidsController(kidServiceMock.Object);
            var response = await kidController.GetKidAync(kidId);
            var status = (OkObjectResult)response.Result;
            Assert.Equal(200, status.StatusCode);
        }
        [Fact]
        public async Task CreateKidAsync_KidAdded_ReturnsSameKidWithId()
        {
            // ARRANGE
            var kidModel = new KidModel()
            {
                FirstName = "Manuel",
                LastName = "Flores",
                CI = "1234567",
                BirthDate = new DateTime(2010, 9, 12),
                ProgramHouse = "SDA",
                BirthPlace = "Cochabamba",
                Gender = "masculino"
            };
            var kidServiceMock = new Mock<IKidService>();
            kidServiceMock.Setup(r => r.CreateKidAsync(kidModel)).ReturnsAsync(new KidModel()
            {
                Id = 1,
                FirstName = "Manuel",
                LastName = "Flores",
                CI = "1234567",
                BirthDate = new DateTime(2010, 9, 12),
                ProgramHouse = "SDA",
                BirthPlace = "Cochabamba",
                Gender = "masculino"
            });

            var kidsController = new KidsController(kidServiceMock.Object);

            // ACT
            var response = await kidsController.CreateKidAsync(kidModel);
            var status = response.Result as CreatedResult;
            var kidCreated = status.Value as KidModel;

            // ASSERT
            Assert.Equal(1, kidCreated.Id);
            Assert.Equal("Manuel", kidCreated.FirstName);
            Assert.Equal(201, status.StatusCode);
        }
        [Fact]
        public async Task GetKids_ReturnAllKids_ReturnStatus()
        {
            var kid1 = new KidModel()
            {
                Id = 1,
                FirstName = "Manuel",
                LastName = "Flores",
                CI = "1234567",
                BirthDate = new DateTime(2010, 9, 12),
                ProgramHouse = "SDA",
                BirthPlace = "Cochabamba",
                Gender = "masculino"
            };

            var kid2 = new KidModel()
            {
                Id = 2,
                FirstName = "Pedro",
                LastName = "Romero",
                CI = "1234568",
                BirthDate = new DateTime(2010, 9, 15),
                ProgramHouse = "SDA",
                BirthPlace = "Cochabamba",
                Gender = "masculino"
            };
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutomapperProfile>());
            var mapper = config.CreateMapper();
            var enumerable = new List<KidModel>() { kid1, kid2 } as IEnumerable<KidModel>;
            var kidServiceMock = new Mock<IKidService>();
            kidServiceMock.Setup(r => r.GetKidsAsync()).ReturnsAsync(enumerable);

            var kidController = new KidsController(kidServiceMock.Object);
            var response = await kidController.GetKidsAync();
            var status = (OkObjectResult)response.Result;
            var kidsList = status.Value as List<KidModel>;
            var countKidsList = kidsList.Count();
            Assert.Equal(2, countKidsList);
            Assert.Equal(200, status.StatusCode);
        }
    }
}
