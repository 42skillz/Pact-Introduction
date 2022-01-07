using System.Collections.Generic;

namespace ProviderSuperHeroesConsumerCharactersPact
{
    internal class VersionTags
    {
        public List<string> ProviderTags { get; set; } = new List<string>();
        public List<string> ConsumerTags { get; set; } = new List<string>();
    }
}