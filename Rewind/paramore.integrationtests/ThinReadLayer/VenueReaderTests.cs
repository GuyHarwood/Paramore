﻿using System;
using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Paramore.Adapters.Infrastructure.Repositories;
using Paramore.Domain.Venues;
using Paramore.Ports.Services.ThinReadLayer;
using Version = Paramore.Adapters.Infrastructure.Repositories.Version;

namespace paramore.integrationtests.ThinReadLayer
{
    [Subject("Check that we can get the venue list out of the thin read layer")]
    public class When_viewing_a_list_of_venues
    {
        private const string TEST_VENUE = "Test Venue";
        private static IRepository<Venue, VenueDocument> repository;
        private static IAmAUnitOfWorkFactory unitOfWorkFactory;
        private static IAmAViewModelReader<VenueDocument> reader; 
        private static IEnumerable<VenueDocument> venues;
        private static Venue venue;
        
        Establish context = () =>
        {
            unitOfWorkFactory = new UnitOfWorkFactory();
            repository = new Repository<Venue, VenueDocument>();
            venue = new Venue(id: new Id(), version: new Version(), venueName: new VenueName(TEST_VENUE));

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                repository.UnitOfWork = unitOfWork;
                repository.Add(venue);
                unitOfWork.Commit();
            }

            reader = new VenueReader(unitOfWorkFactory, false);
        };

        Because of = () => venues = reader.GetAll();

        It should_return_the_test_venue = () => venues.Count(v => v.VenueName == TEST_VENUE).ShouldEqual(1);

        Cleanup cleanDownRepository = () =>
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                repository.UnitOfWork = unitOfWork;
                repository.Delete(venue);
                unitOfWork.Commit();
            }                                                      
        };
    }

    [Subject("When retrieving a venueDocument by its id")]
    public class When_viewing_a_venue_by_id
    {
        private const string TEST_VENUE = "Test Venue";
        private static IRepository<Venue, VenueDocument> repository;
        private static IAmAUnitOfWorkFactory unitOfWorkFactory;
        private static IAmAViewModelReader<VenueDocument> reader; 
        private static Venue venue;
        private static VenueDocument venueDocument;
        
        Establish context = () =>
        {
            unitOfWorkFactory = new UnitOfWorkFactory();
            repository = new Repository<Venue, VenueDocument>();
            venue = new Venue(id: new Id(), version: new Version(), venueName: new VenueName(TEST_VENUE));

            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                repository.UnitOfWork = unitOfWork;
                repository.Add(venue);
                unitOfWork.Commit();
            }

            reader = new VenueReader(unitOfWorkFactory, false);
        };

        Because of = () => venueDocument = reader.Get(venue.Id);

        It should_return_a_venue = () => venueDocument.ShouldNotBeNull();
        It should_return_the_matching_venue = () => venueDocument.Id.ShouldEqual(venue.Id);

        Cleanup cleanDownRepository = () =>
        {
            using (var unitOfWork = unitOfWorkFactory.CreateUnitOfWork())
            {
                repository.UnitOfWork = unitOfWork;
                repository.Delete(venue);
                unitOfWork.Commit();
            }                                                      
        };
    }
}
