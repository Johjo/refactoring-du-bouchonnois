using Domain.Core.Tests.Aggregates;
using FluentAssertions;
using static Domain.Core.Tests.Data.Movies;

namespace Domain.Core.Tests
{
    public class AggregateShould
    {
        private readonly Guid _id;
        private readonly Movie _movie;

        public AggregateShould()
        {
            _id = Guid.NewGuid();
            _movie = Oppenheimer.Movie(_id);
        }

        [Fact]
        public void have_raised_creation_event()
        {
            _movie.HasRaisedEvent(new MovieCreated(_id, Data.Now, Oppenheimer.Title, Oppenheimer.ReleaseDate))
                .Should()
                .BeTrue();
            _movie.Version.Should().Be(1);
            _movie.Id.Should().Be(_id);
        }

        [Fact]
        public void have_raised_casting_changed_event()
        {
            var newCasting = new List<string> {"Cillian Murphy", "Florence Pugh"}.ToSeq();

            _movie.ChangeCast(newCasting);

            _movie.HasRaisedEvent(new CastingHasChanged(_id, Data.Now, newCasting))
                .Should()
                .BeTrue();

            _movie.Version.Should().Be(2);
        }

        [Fact]
        public void throw_handler_not_found_when_apply_method_not_defined()
        {
            var act = () => _movie.NotWellImplementedBehavior();
            act.Should()
                .Throw<HandlerForDomainEventNotFoundException>()
                .WithMessage(
                    "Aggregate of type 'Movie' raised an event of type 'NotWellImplementedDomainBehaviorRaised' but no handler could be found to handle the event.");
        }

        [Fact]
        public void implement_value_equality_on_id()
        {
            _movie.Equals(new Movie(Guid.NewGuid(), Data.TimeProvider, Oppenheimer.Title, Oppenheimer.ReleaseDate))
                .Should()
                .BeFalse();

            _movie.Equals(new Movie(_id, Data.TimeProvider, Oppenheimer.Title, Oppenheimer.ReleaseDate))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void return_false_on_equals_null()
        {
            _movie.Equals(null)
                .Should()
                .BeFalse();
        }
    }
}