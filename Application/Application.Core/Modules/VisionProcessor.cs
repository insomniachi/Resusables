﻿using System;
using System.Collections.Generic;
using KEI.Infrastructure.Configuration;
using KEI.Infrastructure.Service;
using KEI.Infrastructure.Validation;
using Application.Core.Constants;
using Application.Core.Interfaces;
using static Application.Core.Constants.ConfigKeys;
using KEI.Infrastructure.Events;

namespace Application.Core.Modules
{
    public class VisionProcessor : ConfigHolder, IVisionProcessor
    {

        #region GenericConfigHolder Members
        public override string ConfigPath => @"Configs\vision.xcfg";
        public override string ConfigName => "VISION";
        protected override PropertyContainerBuilder DefineConfigShape()
        {
            return PropertyContainerBuilder.Create(ConfigName, ConfigPath)
                .WithProperty("MorphologyOperatorSize", 25, validation: ValidationBuilder.Create().Positive())
                .WithProperty("MedinBlurSize", 3, validation: ValidationBuilder.Create().Positive())
                .WithProperty("WindowSizeTolerance", 110, validation: ValidationBuilder.Create().Positive())
                .WithProperty("PermissibleDistanceOfRotation", 100, validation: ValidationBuilder.Create().Positive());
        }

        #endregion

        private IPropertyContainer _recipe;
        #region Constructor
        
        public VisionProcessor(IEssentialServices essentialServices) : base(essentialServices)
        {
            _eventAggregator.GetEvent<RecipeLoadedEvent>().Subscribe(recipe => _recipe = recipe);
        }

        #endregion

        #region IVisionProcessor Members
        public List<TestResult> ReferenceTest(string imagePath)
        {
            _logger.Debug($"Entering {nameof(ReferenceTest)}");

            int morphSize = 25;
            GetValue(VisionConfigKeys.MorphologyOperatorSize, ref morphSize);

            int medianBlurSize = 3;
            GetValue(VisionConfigKeys.MedinBlurSize, ref medianBlurSize);

            int windowSizeTolarance = 110;
            GetValue(VisionConfigKeys.WindowSizeTolerance, ref windowSizeTolarance);

            int maximumPermissibleDistance = 100;
            GetValue(VisionConfigKeys.PermissibleDistanceOfRotation, ref maximumPermissibleDistance);

            // TODO :: Create dummy Function

            //

            var random = new Random();
            var results = new List<TestResult>();
            for (int i = 0; i < 5; i++)
            {
                var result = new TestResult
                {
                    ID = $"P{i + 1}",
                    AverageIntensity = 38000000 + random.NextDouble() * 10000000,
                    AverageIntensityReference = 38000000 + random.NextDouble() * 10000000,
                    IsPass = true
                };
                results.Add(result);
            }


            _logger.Debug($"Leaving {nameof(ReferenceTest)}");

            return results;
        }

        public List<TestResult> Test(string imagePath)
        {
            _logger.Debug($"Entering {nameof(Test)}");

            double maxTransmittance = 100;
            double minTransmittance = 90;
            _recipe?.Get(RecipeKeys.MaxTransmittance, ref maxTransmittance);
            _recipe?.Get(RecipeKeys.MinTransmittance, ref minTransmittance);

            int morphSize = 25;
            GetValue(VisionConfigKeys.MorphologyOperatorSize, ref morphSize);

            int medianBlurSize = 3;
            GetValue(VisionConfigKeys.MedinBlurSize, ref medianBlurSize);

            int windowSizeTolarance = 110;
            GetValue(VisionConfigKeys.WindowSizeTolerance, ref windowSizeTolarance);

            int maximumPermissibleDistance = 100;
            GetValue(VisionConfigKeys.PermissibleDistanceOfRotation, ref maximumPermissibleDistance);

            // TODO :: Create dummy Function

            //

            var random = new Random();
            var results = new List<TestResult>();
            for (int i = 0; i < 5; i++)
            {
                var result = new TestResult
                {
                    ID = $"P{i + 1}",
                    AverageIntensity = 38000000 + random.NextDouble() * 10000000,
                    AverageIntensityReference = 38000000 + random.NextDouble() * 10000000
                };

                result.Transmittance = (result.AverageIntensity / result.AverageIntensityReference) * 100;
                result.IsPass = Validators.Range(minTransmittance, maxTransmittance).Validate(result.Transmittance).IsValid;

                results.Add(result);
            }


            _logger.Debug($"Leaving {nameof(Test)}");

            return results;
        }

        #endregion
    }
}
