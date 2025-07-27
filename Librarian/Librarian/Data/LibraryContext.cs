using Librarian.Models;
using Microsoft.EntityFrameworkCore;

namespace Librarian.Data
{
    public class LibraryContext : DbContext
    {
        public DbSet<LibraryResource> Resources { get; set; }
        public DbSet<BorrowRecord> BorrowRecords { get; set; }
        public DbSet<ReadingSession> ReadingSessions { get; set; }

        public LibraryContext(DbContextOptions<LibraryContext> options)
            : base(options) { }

        // parameterless constructor for design-time
        public LibraryContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                // default connection string for sql server localdb
                optionsBuilder.UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=LibraryDB;Trusted_Connection=true;MultipleActiveResultSets=true"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // configure resource entity
            modelBuilder.Entity<LibraryResource>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Author).IsRequired().HasMaxLength(150);
                entity.Property(e => e.ISBN).HasMaxLength(50);
                entity.Property(e => e.Genre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Publisher).HasMaxLength(200);
                entity.Property(e => e.Language).HasMaxLength(50).HasDefaultValue("English");
                entity.Property(e => e.PublicationYear).IsRequired();
                entity.Property(e => e.IsAvailable).IsRequired().HasDefaultValue(true);
                entity.Property(e => e.Type).IsRequired();
                entity.Property(e => e.Description).HasColumnType("nvarchar(max)");
                entity.Property(e => e.Content).HasColumnType("nvarchar(max)");
                entity.Property(e => e.ContentPreview).HasMaxLength(1000);
                entity.Property(e => e.CoverImagePath).HasMaxLength(500);
                entity.Property(e => e.Rating).HasColumnType("decimal(3,2)");
                entity.Property(e => e.CreatedDate).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.LastModified).IsRequired().HasDefaultValueSql("GETDATE()");
            });

            // configure borrow record entity
            modelBuilder.Entity<BorrowRecord>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BorrowerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.BorrowDate).IsRequired();
                entity.Property(e => e.DueDate).IsRequired();
                entity.Property(e => e.ReturnDate);
                entity.Property(e => e.IsReturned).IsRequired();

                // foreign key relationship
                entity
                    .HasOne(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // configure reading session entity
            modelBuilder.Entity<ReadingSession>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ReaderName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.StartTime).IsRequired().HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.CurrentPage).IsRequired().HasDefaultValue(1);
                entity.Property(e => e.TotalPages).IsRequired();
                entity.Property(e => e.IsCompleted).IsRequired().HasDefaultValue(false);
                entity.Property(e => e.Notes).HasColumnType("nvarchar(max)");

                // foreign key relationship
                entity
                    .HasOne(e => e.Resource)
                    .WithMany()
                    .HasForeignKey(e => e.ResourceId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // seed library with diverse content - 10 books, 4 journals, 5 movies
            modelBuilder
                .Entity<LibraryResource>()
                .HasData(
                    // books section - fantasy, superhero, reference
                    new LibraryResource
                    {
                        Id = 1,
                        Title = "The Young Wizard's Journey",
                        Author = "A. Fantasy",
                        ISBN = "978-1-234-56789-0",
                        PublicationYear = 2021,
                        Genre = "Fantasy",
                        Publisher = "Magic Books Ltd",
                        PageCount = 320,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "An original story about a young person discovering their magical abilities at a school for wizards.",
                        ContentPreview =
                            "Alex had always known they were different. Strange things happened around them - objects moved without being touched, lights flickered when they were upset, and sometimes they could swear they heard whispers from empty rooms. But nothing could have prepared them for the letter that arrived on their eleventh birthday...",
                        Content =
                            "Chapter 1: The Letter That Changed Everything\n\nAlex had always known they were different. Strange things happened around them - objects moved without being touched, lights flickered when they were upset, and sometimes they could swear they heard whispers from empty rooms. But nothing could have prepared them for the letter that arrived on their eleventh birthday.\n\nThe envelope was made of thick, cream-colored parchment, and it was addressed in emerald green ink to 'Alex Morgan, The Small Bedroom, 4 Privet Lane.' No postage stamp, no return address, just those precise, flowing letters that seemed to shimmer in the morning light.\n\n'Who's this from?' Alex's aunt Martha asked suspiciously, snatching the letter before Alex could open it. She examined it carefully, turning it over in her hands. 'Probably some prank from those neighborhood kids.'\n\nBut when she tried to open it, the envelope seemed to seal itself shut, as if held by invisible glue. She pulled and tugged, but it wouldn't budge. 'Ridiculous,' she muttered, handing it back to Alex with a frown.\n\nThe moment Alex touched the envelope, it opened easily, as if it had been waiting for them all along. Inside was a letter written on the same cream parchment:\n\nDear Alex,\n\nWe are pleased to inform you that you have been accepted at the Arcane Academy of Magical Arts. Please find enclosed a list of all necessary books and equipment. Term begins on September 1st. We await your owl by no later than July 31st.\n\nYours sincerely,\nProfessor McGillicuddy\nDeputy Headmaster\n\nAlex read the letter three times, their heart pounding with excitement and disbelief. Magic was real? They were a wizard? It explained so much about the strange occurrences throughout their life.\n\n'What does it say?' Aunt Martha demanded, but when Alex showed her the letter, all she saw was a blank piece of parchment. 'Very funny, Alex. Stop wasting time with silly pranks and go do your chores.'\n\nBut Alex knew this was no prank. This was the beginning of the greatest adventure of their life...",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 2,
                        Title = "Heroes United: The Battle for Earth",
                        Author = "Stan L. Marvel",
                        ISBN = "978-0-987-65432-1",
                        PublicationYear = 2023,
                        Genre = "Superhero Fiction",
                        Publisher = "Hero Comics Press",
                        PageCount = 450,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "When an alien threat emerges, Earth's mightiest heroes must unite to defend humanity. Action-packed adventure featuring Iron Knight, Captain Liberty, Thunder God, and the Incredible Green Guardian.",
                        ContentPreview =
                            "The sky over New York City crackled with otherworldly energy as massive ships descended through the clouds. Citizens fled in terror as strange beings in gleaming armor poured from the vessels, their weapons unlike anything Earth had ever seen. This was the moment the world's greatest heroes had trained for...",
                        Content =
                            "Chapter 1: The Invasion Begins\n\nThe sky over New York City crackled with otherworldly energy as massive ships descended through the clouds. Citizens fled in terror as strange beings in gleaming armor poured from the vessels, their weapons unlike anything Earth had ever seen. This was the moment the world's greatest heroes had trained for, though none had imagined it would come so suddenly.\n\nTony Starke was in his workshop when FRIDAY alerted him to the situation. 'Sir, we have multiple unidentified aircraft over Manhattan. Energy signatures are off the charts.'\n\n'Show me,' Tony commanded, his workshop screens lighting up with satellite feeds. What he saw made his blood run cold. 'FRIDAY, get me Steve Rogers. Now.'\n\nAcross the city, Steve Rogers was leading a training session when his communicator buzzed. The moment he saw the footage, he knew their greatest test had arrived. 'Assemble the team,' he ordered. 'This is what we've prepared for.'\n\nMeanwhile, in Asgard, Thor sensed the disturbance across the realms. The Bifrost bridge hummed with power as he prepared to return to Midgard. His friends would need him now more than ever.\n\nDr. Bruce Banner was lecturing at Columbia University when the first explosions rocked the city. Through the classroom windows, he could see the alien ships unleashing devastating energy beams on the streets below. His students screamed and fled, but Bruce remained calm. He knew what had to be done.\n\n'Everyone stay calm and follow evacuation procedures,' he announced to his class. But even as he spoke, he could feel the familiar stirring within him. The other guy was ready to come out and play.\n\nNatasha Romanoff was already in position, having been tracking unusual energy readings for the past week. From her vantage point on a Manhattan rooftop, she had a clear view of the invasion force. Her earpiece crackled to life as Nick Fury's voice cut through the chaos.\n\n'Romanoff, we need you to coordinate ground evacuation while the others engage the primary threat.'\n\n'Copy that,' she replied, already moving. 'But sir, this is bigger than anything we've faced before. We're going to need everyone.'\n\nAs the heroes converged on the battlefield, each brought their unique skills to bear against an enemy unlike any they had faced. The fate of humanity hung in the balance, and only by working together could they hope to prevail...",
                        Rating = 4.9m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 3,
                        Title = "Wizarding World: Magical Creatures Encyclopedia",
                        Author = "Luna S. Lovegood",
                        ISBN = "978-2-345-67890-1",
                        PublicationYear = 2022,
                        Genre = "Fantasy Reference",
                        Publisher = "Diagon Alley Books",
                        PageCount = 680,
                        Type = ResourceType.Book,
                        IsAvailable = false, // currently borrowed by alex
                        Description =
                            "A comprehensive guide to magical creatures, their habitats, behaviors, and care. Essential reading for any aspiring magizoologist or curious wizard.",
                        ContentPreview =
                            "Dragons have captivated the magical world for millennia. These magnificent beasts, with their ancient wisdom and fearsome power, represent both the beauty and danger of magic itself. The most common species encountered in Britain is the Common Welsh Green...",
                        Content =
                            "Chapter 12: Dragons of the British Isles\n\nDragons have captivated the magical world for millennia. These magnificent beasts, with their ancient wisdom and fearsome power, represent both the beauty and danger of magic itself. The most common species encountered in Britain is the Common Welsh Green, though several other varieties have been documented.\n\nThe Common Welsh Green (Draco britannicus) is perhaps the most docile of all dragon species, preferring to nest in the mountainous regions of Wales. Adults typically reach lengths of 18-20 feet, with a wingspan nearly double that measurement. Their distinctive emerald scales shimmer with an inner fire, and their eyes burn with an intelligence that has led many researchers to believe dragons possess a form of magic far older than our own.\n\nUnlike their more aggressive cousins, Welsh Greens rarely attack humans unless directly threatened or protecting their young. They feed primarily on sheep and cattle, though they have been known to supplement their diet with fish from mountain lakes. Their flame burns a brilliant green-white and can reach temperatures of over 2,000 degrees Celsius.\n\nThe Norwegian Ridgeback (Draco norwegicus) is considerably more dangerous. These dragons are aggressive by nature and will attack without provocation. They are easily identified by the prominent spines along their backs and their distinctive bronze coloration. Ridgebacks are native to the fjords of Norway but have been spotted as far south as Scotland in recent years.\n\nOne of the most fascinating aspects of dragon behavior is their hoarding instinct. All dragons, regardless of species, are compelled to collect objects they find beautiful or valuable. These hoards can contain anything from precious metals and gems to seemingly worthless trinkets that have caught the dragon's fancy. Some scholars believe this behavior is linked to their magical nature, as many of the items in dragon hoards have been found to possess unique magical properties.\n\nPerhaps the most dangerous dragon species in Europe is the Hungarian Horntail (Draco hungaricus). These black-scaled beasts are known for their aggression and their particular fondness for hunting wizards. Their tail spikes can pierce dragon-hide armor, and their flame burns so hot it appears almost transparent. Horntails are fiercely territorial and will defend their nesting grounds to the death.\n\nDragon eggs are among the most regulated magical substances in the world. The sale, purchase, or possession of dragon eggs without proper Ministry authorization carries a sentence of life in Azkaban. This is not merely due to their value (a single dragon egg can be worth more than most wizards earn in a lifetime) but because of the extreme danger posed by baby dragons.\n\nNewborn dragons, or dragonets, are born with their full magical abilities intact. They can breathe fire within hours of hatching and will instinctively attack anything they perceive as a threat. Many well-meaning wizards have lost their lives attempting to raise dragons from eggs, underestimating the lethal nature of these creatures even in infancy.\n\nFor those brave or foolish enough to encounter a dragon in the wild, remember these essential survival tips: Never make direct eye contact, as dragons consider this a challenge. Move slowly and avoid sudden movements. Most importantly, never attempt to steal from a dragon's hoard - they can sense the magical signature of their possessions from miles away and will hunt the thief relentlessly.\n\nDragons remain one of the most magnificent and terrifying creatures in our magical world, deserving of both our respect and our caution...",
                        Rating = 4.7m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 4,
                        Title = "Superhero Training Academy: Complete Season 1",
                        Author = "Marvel Studios Academy",
                        ISBN = "DVD-HERO-2024",
                        PublicationYear = 2024,
                        Genre = "Superhero Documentary",
                        Publisher = "Hero Education Media",
                        PageCount = 600,
                        Type = ResourceType.Media,
                        IsAvailable = true,
                        Description =
                            "Follow new recruits as they train to become the next generation of superheroes. Features exclusive interviews with legendary heroes and behind-the-scenes training footage.",
                        ContentPreview =
                            "Welcome to the Superhero Training Academy, where ordinary individuals discover their extraordinary potential. In this exclusive documentary series, we follow twelve recruits through their intensive training program designed to prepare them for the responsibilities of heroism...",
                        Content =
                            "Episode 1: Welcome to Hero Academy\n\nWelcome to the Superhero Training Academy, where ordinary individuals discover their extraordinary potential. In this exclusive documentary series, we follow twelve recruits through their intensive training program designed to prepare them for the responsibilities of heroism.\n\nOur first recruit is Sarah Chen, a 23-year-old engineer from Seattle who discovered her ability to manipulate electromagnetic fields during a power outage at her office building. 'I thought I was going crazy,' Sarah explains during her interview. 'Electronics would malfunction around me when I got stressed. Then one day, I accidentally brought down the entire city grid during a panic attack. That's when S.H.I.E.L.D. found me.'\n\nTraining Director Maria Hill explains the Academy's philosophy: 'Having powers doesn't make you a hero. It's what you choose to do with those powers that counts. Our program is designed to teach discipline, responsibility, and most importantly, the moral framework necessary to protect innocent lives.'\n\nThe Academy's facilities are impressive - a converted military base in upstate New York featuring state-of-the-art training equipment, simulation chambers, and specialized environments for testing different types of abilities. The main training hall is a massive warehouse equipped with obstacle courses, combat training areas, and holographic projection systems capable of creating realistic disaster scenarios.\n\nRecruit Marcus Thompson, who possesses enhanced strength and durability, struggles with controlling his power during combat training. 'I keep breaking the equipment,' he admits after accidentally punching through a reinforced concrete wall. 'Captain Rogers keeps telling me it's not about being strong - it's about knowing when not to use that strength.'\n\nThe psychological aspects of hero training are just as important as the physical ones. Dr. Helen Cho, the Academy's chief psychologist, works with recruits to address the mental challenges of heroism. 'Many of our students have experienced trauma related to their abilities,' she explains. 'Some have accidentally hurt people they care about. Others have been rejected by friends and family. We help them process these experiences and build the emotional resilience necessary for heroic work.'\n\nPerhaps the most challenging aspect of the program is the ethics curriculum, led by Professor Charles Xavier via holographic link from his school in Westchester. 'Every hero will face impossible choices,' Xavier explains to the recruits during a particularly intense session. 'The question isn't whether you can save everyone - it's how you decide who to save when you can't save them all.'\n\nThe episode concludes with the recruits' first major test - a simulated terrorist attack on a crowded shopping mall. Using holographic technology and advanced robotics, the training staff creates a scenario where recruits must work together to neutralize threats while protecting civilians. The results are mixed, with several recruits making critical errors that would have resulted in civilian casualties in a real situation.\n\n'This is why we train,' Director Hill reminds them during the debriefing. 'Every mistake you make here is a life you'll save in the field. The stakes couldn't be higher, and neither could the honor of what you're preparing to do.'\n\nNext week: The recruits face their first real-world assignment when a natural disaster strikes nearby, and they must put their training to the test alongside established heroes...",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 5,
                        Title = "The Dark Arts: A History of Forbidden Magic",
                        Author = "Severus P. Snape",
                        ISBN = "978-3-456-78901-2",
                        PublicationYear = 2020,
                        Genre = "Dark Fantasy",
                        Publisher = "Borgin & Burkes Academic",
                        PageCount = 890,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "A comprehensive examination of dark magic throughout history. Required reading for advanced Defense Against the Dark Arts students. Contains detailed analysis of curses, hexes, and protective countermeasures.",
                        ContentPreview =
                            "The study of dark magic is not for the faint of heart. Those who delve into these forbidden arts must understand that knowledge itself can be a dangerous weapon. This text serves as both warning and guide for those who must understand darkness to defend against it...",
                        Content =
                            "Preface: On the Nature of Dark Magic\n\nThe study of dark magic is not for the faint of heart. Those who delve into these forbidden arts must understand that knowledge itself can be a dangerous weapon. This text serves as both warning and guide for those who must understand darkness to defend against it.\n\nI have spent decades studying the dark arts, not out of fascination with evil, but from the necessity of understanding one's enemy. As a professor of Defense Against the Dark Arts, I have seen firsthand the devastation that can be wrought by those who embrace the darkness without understanding its true cost.\n\nDark magic, at its core, is magic that is performed with the intention to harm, control, or corrupt. It feeds on negative emotions - hatred, fear, anger, despair. The more intense these emotions, the more powerful the magic becomes. This is why dark wizards often appear to grow stronger over time; they become addicted to the rush of power that comes from channeling their darkest impulses.\n\nThe Three Unforgivable Curses represent the pinnacle of dark magic, each designed to strip away fundamental human rights. The Imperius Curse removes free will, turning the victim into a puppet. The Cruciatus Curse inflicts unbearable pain while leaving no physical marks, making it a favorite tool of interrogators and sadists. And the Killing Curse... Avada Kedavra ends life itself with no possibility of magical healing or reversal.\n\nWhat makes these curses truly unforgivable is not just their effects, but what they require of the caster. To successfully perform any of these curses, one must mean them completely. There can be no hesitation, no moral qualms, no trace of humanity left in the moment of casting. The wizard must embrace the darkness fully, and in doing so, they lose a piece of their soul forever.\n\nThis is the true danger of dark magic - not just what it does to the victim, but what it does to the caster. Each act of dark magic leaves a mark on the soul, a corruption that grows with every subsequent act. It becomes easier each time, until eventually, the wizard loses the ability to feel empathy, love, or remorse. They become something less than human.\n\nVoldemort serves as the ultimate example of this corruption. Born Tom Marvolo Riddle, he was once a brilliant student, charming and capable. But his obsession with dark magic, his quest for immortality through the creation of Horcruxes, transformed him into something monstrous. By the time of his downfall, he had become barely recognizable as having once been human.\n\nYet we must study these arts, for only by understanding darkness can we hope to defend against it. The protective spells, the counter-curses, the methods of detection - all require knowledge of what we seek to protect against. A healer must understand poison to create antidotes; a defender must understand dark magic to create protections.\n\nThis book contains knowledge that has been carefully compiled over centuries, drawn from the most dangerous texts in wizarding libraries, from the confessions of captured dark wizards, and from my own observations during the wars against Voldemort. I have included detailed instructions for defensive spells, methods for detecting dark magic, and the psychological profiles of those who turn to darkness.\n\nTo my students, I offer this warning: knowledge is power, but power without wisdom is destruction. Study these pages well, but never forget the human cost of the magic described within. The moment you begin to see these spells as solutions rather than problems, you have already begun your journey into the darkness.\n\nRemember always that the brightest light casts the darkest shadow, and those who fight monsters must be careful not to become monsters themselves.\n\n- Professor S. Snape\n  Hogwarts School of Witchcraft and Wizardry",
                        Rating = 4.3m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    // journals
                    new LibraryResource
                    {
                        Id = 6,
                        Title = "Journal of Advanced Magical Theory",
                        Author = "Albus P. Dumbledore",
                        ISBN = "ISSN-2045-7890",
                        PublicationYear = 2024,
                        Genre = "Academic Journal",
                        Publisher = "Wizarding Academic Press",
                        PageCount = 45, // actual page count based on content
                        Type = ResourceType.Journal,
                        IsAvailable = false, // currently borrowed by ashu
                        Description =
                            "Leading academic journal featuring cutting-edge research in magical theory, spell innovation, and theoretical frameworks for understanding magic.",
                        ContentPreview =
                            "This month's edition explores the revolutionary concept of 'magic as information theory' - suggesting that spells are essentially data structures that interact with the fundamental fabric of reality...",
                        Content =
                            "Volume 127, Issue 3 - March 2024\n\nEditorial: Magic as Information Theory\nBy Professor Hermione Granger-Weasley\n\nThis month's edition explores the revolutionary concept of 'magic as information theory' - suggesting that spells are essentially data structures that interact with the fundamental fabric of reality. Dr. Luna Lovegood's groundbreaking research at the Department of Mysteries has opened new avenues for understanding how magical energy can be quantified, stored, and transmitted.\n\nFeatured Articles:\n\n1. 'Quantum Entanglement in Paired Wands: A Study of the Elder Wand Phenomenon'\nBy Garrick Ollivander III\n\nOur research into wand cores has revealed fascinating parallels between magical resonance and quantum mechanical principles. When two wands share material from the same magical creature, they exhibit properties remarkably similar to quantum entanglement. This study follows 847 paired wands over a 50-year period, documenting instances of sympathetic magical discharge and cross-wand spell interference.\n\nThe implications are staggering. If wands can indeed maintain quantum-level connections regardless of distance, this could revolutionize long-distance magical communication, defensive ward networks, and even our understanding of apparition itself.\n\n2. 'Temporal Magic: New Theories on Time-Turner Mechanics'\nBy Dr. Minerva McGonagall\n\nFollowing the destruction of the Time Room at the Ministry of Magic, researchers have been working to understand how time-manipulation devices actually function. Our analysis suggests that Time-Turners don't actually send the user backward in time, but rather create localized temporal bubbles where time flows at different rates.\n\nThis theory explains several previously inexplicable phenomena, including why time-travelers can interact with their past selves without creating paradoxes, and why extended time-turner use leads to rapid aging. The temporal bubble theory suggests that the user experiences every moment of 'rewound' time, but in an accelerated state that appears instantaneous to outside observers.\n\n3. 'Magical Creature Intelligence: Beyond Human Understanding'\nBy Luna S. Lovegood\n\nOur five-year study of dragon behavior has revealed cognitive abilities that challenge our assumptions about magical creature intelligence. Dragons demonstrate not only problem-solving skills equivalent to human eight-year-olds, but also show evidence of abstract thinking, long-term planning, and what can only be described as artistic expression.\n\nThe most remarkable discovery came when we observed a group of Welsh Greens arranging their hoard items in complex geometric patterns that correspond to ancient magical symbols. When replicated by human wizards, these patterns produced powerful protective enchantments that we had previously thought impossible.\n\nThis suggests that magical creatures may possess intuitive understanding of magical principles that exceeds human knowledge, raising profound questions about the nature of intelligence itself.\n\nBook Reviews:\n\n'Elementary Spell Theory Revised' by Miranda Goshawk\nReviewed by Professor Filius Flitwick\n\nGoshawk's latest edition incorporates recent discoveries about wand movement optimization and incantation phonetics. Her analysis of how regional accents affect spell potency is particularly enlightening for international magical education programs.\n\nUpcoming Conferences:\n\n- International Symposium on Magical Ethics (June 15-17, Edinburgh)\n- Conference on Experimental Transfiguration (July 22-24, Salem)\n- Annual Meeting of the Society for Magical Creature Rights (August 5-7, Brussels)\n\nCall for Papers:\n\nThe Journal seeks submissions for our special October issue on 'Magic and Technology Integration.' We're particularly interested in research on magical enhancement of Muggle technology and the development of hybrid magical-electronic devices.\n\nSubmission deadline: May 31, 2024\nFor submission guidelines, contact the editorial board at editor@advancedmagicaltheory.org\n\n---\n\nSubscription Information:\nAnnual subscription (12 issues): 50 Galleons\nStudent rate: 25 Galleons with valid school ID\nLibrary institutional rate: 200 Galleons\n\nISSN: 2045-7890\nPublished monthly by Wizarding Academic Press, Diagon Alley, London",
                        Rating = 4.6m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 7,
                        Title = "Modern Defense Strategies Quarterly",
                        Author = "Harry J. Potter",
                        ISBN = "ISSN-1987-6543",
                        PublicationYear = 2024,
                        Genre = "Military Journal",
                        Publisher = "Auror Training Institute",
                        PageCount = 38,
                        Type = ResourceType.Journal,
                        IsAvailable = true,
                        Description =
                            "Quarterly publication focusing on modern magical defense techniques, threat assessment, and protection strategies for both military and civilian applications.",
                        ContentPreview =
                            "The evolution of dark magic requires constant adaptation of our defensive strategies. This issue examines emerging threats from techno-magical hybrid attacks and the rise of AI-assisted dark magic...",
                        Content =
                            "Issue 45, Q1 2024\n\nEditor's Note\nBy Harry Potter, Senior Auror\n\nThe evolution of dark magic requires constant adaptation of our defensive strategies. This issue examines emerging threats from techno-magical hybrid attacks and the rise of AI-assisted dark magic detection systems.\n\nLead Article: 'Cyber-Magic: When Technology Meets Dark Arts'\nBy Kingsley Shacklebolt, Minister of Magic\n\nThe integration of Muggle technology with magical attacks represents a new frontier in dark arts evolution. Recent incidents involving magically enhanced computer viruses and technologically amplified curses have forced a complete reevaluation of our defensive protocols.\n\nCase Study 1: The London Bridge Incident\nLast September, dark wizards successfully infiltrated London's traffic control systems, using a combination of Confundus Charms and computer programming to cause massive gridlock while they robbed Gringotts Bank. The attack demonstrated how modern dark wizards are adapting to our increasingly connected world.\n\nTraditional magical defenses proved insufficient against this hybrid threat. Anti-technology jinxes couldn't affect the properly shielded computer systems, while Muggle cybersecurity measures were useless against the magical components of the attack.\n\nRecommended Countermeasures:\n- Development of hybrid detection systems combining magical sensors with electronic monitoring\n- Cross-training programs for Aurors in both magical and Muggle security techniques\n- Establishment of joint task forces with Muggle intelligence agencies\n\nSpecial Report: 'AI in Magical Law Enforcement'\nBy Dr. Cho Chang, Department of Magical Research\n\nArtificial Intelligence systems are revolutionizing threat detection in the magical world. The new ATHENA system (Automated Threat Hexing and Enchantment Network Analyzer) can process thousands of magical signatures simultaneously, identifying potential dark magic signatures in real-time.\n\nField test results from the past six months show a 347% improvement in early threat detection compared to human-only monitoring systems. The AI can recognize patterns that human analysts miss, such as subtle variations in magical energy that indicate curse preparation.\n\nHowever, AI systems remain vulnerable to magical interference. The Imperius Curse has proven particularly effective against AI decision-making algorithms, requiring the development of new protective protocols.\n\nTactical Review: 'Urban Magical Combat'\nBy Neville Longbottom, Defense Against the Dark Arts Professor\n\nModern magical conflicts increasingly occur in densely populated Muggle areas, requiring new tactical approaches that minimize civilian exposure while maintaining operational effectiveness.\n\nKey considerations for urban magical combat:\n\n1. Concealment Protocols: Traditional Disillusionment Charms are insufficient in areas with extensive CCTV coverage. New multi-spectrum concealment spells are required.\n\n2. Collateral Damage Minimization: Precision curse work is essential. Wide-area spells like Fiendfyre are absolutely prohibited in urban environments.\n\n3. Muggle Memory Management: Large-scale memory modification operations require careful coordination with the Department for Regulation and Control of Magical Creatures' Obliviation Task Force.\n\nTraining Corner: 'Advanced Shield Charm Techniques'\nBy Professor Luna Lovegood\n\nRecent innovations in shield charm theory have produced remarkable improvements in defensive capability. The new 'adaptive shielding' technique allows shields to automatically adjust their properties based on incoming threats.\n\nPractice Exercise: Gradient Shield Training\nSet up targets at varying distances (5m, 10m, 20m). Cast Protego while visualizing the shield adapting to different threat types:\n- Physical projectiles: Dense, rigid barrier\n- Curse fire: Heat-dissipating mesh structure  \n- Mental intrusion: Reflective surface with built-in confusion elements\n\nMaster this technique requires approximately 200 hours of practice, but the results justify the investment. Advanced practitioners report 85% effectiveness against unknown threats.\n\nEquipment Review: Latest Protective Gear\n- Dragon-hide armor with integrated cooling charms: 4/5 stars\n- Self-sharpening silver daggers for werewolf encounters: 5/5 stars\n- Portable ward anchors for rapid perimeter establishment: 3/5 stars (reliability issues)\n\nUpcoming Training Opportunities:\n- Advanced Dueling Workshop (Ministry Atrium, March 15)\n- Anti-Dark Arts Refresher Course (Hogwarts, April 1-3)\n- International Defense Conference (Paris, May 20-22)\n\nSubmit articles to: editorial@moderndefense.gov.magic\nNext issue theme: 'Magical Terrorism Prevention'\nSubmission deadline: March 30, 2024",
                        Rating = 4.7m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    // real movies on disks/cds
                    new LibraryResource
                    {
                        Id = 8,
                        Title = "The Avengers",
                        Author = "Marvel Studios",
                        ISBN = "DVD-MCU-2012-001",
                        PublicationYear = 2012,
                        Genre = "Action/Superhero",
                        Publisher = "Walt Disney Studios",
                        PageCount = 8, // based on special features content
                        Type = ResourceType.Media,
                        IsAvailable = true,
                        Description =
                            "Earth's Mightiest Heroes must come together and learn to fight as a team to stop the mischievous Loki and his alien army from enslaving humanity. Special 2-disc collector's edition with behind-the-scenes content.",
                        ContentPreview =
                            "Disc 1: Feature Film (143 minutes)\nDisc 2: Special Features including deleted scenes, gag reel, and director's commentary...",
                        Content =
                            "THE AVENGERS - Special Edition DVD Collection\n\nDisc 1: Feature Film\nRuntime: 143 minutes\nFormat: Widescreen (2.35:1)\nAudio: English 5.1 Surround, Spanish, French\nSubtitles: English, Spanish, French, Audio Description\n\nWhen an unexpected enemy emerges that threatens global safety and security, Nick Fury, Director of the international peacekeeping agency known as S.H.I.E.L.D., finds himself in need of a team to pull the world back from the brink of disaster. Spanning the globe, a daring recruitment effort begins.\n\nStarring:\n- Robert Downey Jr. as Tony Stark/Iron Man\n- Chris Evans as Steve Rogers/Captain America  \n- Mark Ruffalo as Bruce Banner/Hulk\n- Chris Hemsworth as Thor\n- Scarlett Johansson as Natasha Romanoff/Black Widow\n- Jeremy Renner as Clint Barton/Hawkeye\n- Tom Hiddleston as Loki\n- Samuel L. Jackson as Nick Fury\n\nDisc 2: Special Features\n\n'Marvel One-Shot: Item 47' (12 minutes)\nFollow the story of what happened to one of the Chitauri weapons left behind after the Battle of New York.\n\n'A Visual Journey' (22 minutes)\nDirector Joss Whedon and Marvel Studios President Kevin Feige discuss the challenges of bringing together multiple superhero franchises into one cohesive film.\n\n'The Avengers Initiative: A Marvel Second Screen Experience'\nInteractive content that synchronizes with the film for behind-the-scenes information, character profiles, and trivia.\n\nDeleted/Extended Scenes (15 minutes):\n- Extended Steve Rogers gym scene\n- Alternate opening with Maria Hill\n- Additional Coulson/Fury conversation\n- Extended Thor/Loki confrontation\n\nGag Reel (4 minutes)\nHilarious outtakes and bloopers from the cast during filming.\n\nDirector's Audio Commentary\nJoss Whedon provides insight into his creative process, the challenges of ensemble filmmaking, and easter eggs hidden throughout the film.\n\n'Soundscapes: The Music of The Avengers' (8 minutes)\nComposer Alan Silvestri discusses creating the epic orchestral score and developing individual themes for each hero.\n\nConcept Art Gallery\nOver 100 pieces of concept art showing the evolution of character designs, set pieces, and the massive final battle sequence.\n\nMarketing Archive:\n- Theatrical trailers (3)\n- TV spots (12)\n- International promotional materials\n- Poster gallery\n\nEaster Eggs:\n- Hidden Shawarma restaurant locations in menu navigation\n- Stan Lee cameo compilation\n- Marvel Studios logo evolution\n\nBonus Digital Copy Included\nRedeem code for digital download compatible with iTunes, Amazon Prime, Google Play, and VUDU.\n\nSpecial Packaging:\n- Embossed slipcover with holographic Avengers logo\n- 32-page photo booklet with production stills\n- Exclusive character art cards (6)\n\nLanguage Options:\nAudio: English, Spanish (Latin America), French (Canada)\nSubtitles: English SDH, Spanish, French\n\nSystem Requirements:\nRegion 1 DVD player or computer with DVD drive\nCompatible with all standard DVD players worldwide\n\nRated PG-13 for intense sequences of sci-fi violence and action throughout, and a mild language.\n\nCopyright 2012 Marvel Studios. All rights reserved.",
                        Rating = 4.9m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 9,
                        Title = "The Dark Knight",
                        Author = "Warner Bros. Pictures",
                        ISBN = "DVD-DC-2008-002",
                        PublicationYear = 2008,
                        Genre = "Action/Crime/Drama",
                        Publisher = "Warner Home Video",
                        PageCount = 12,
                        Type = ResourceType.Media,
                        IsAvailable = false, // currently borrowed by ashu (overdue)
                        Description =
                            "Batman raises the stakes in his war on crime with the help of Lt. Jim Gordon and Harvey Dent. But they soon find themselves prey to a reign of chaos unleashed by a rising criminal mastermind known as the Joker. 2-Disc Special Edition.",
                        ContentPreview =
                            "Disc 1: Feature Film (152 minutes) + Commentary\nDisc 2: Extensive special features including 'Gotham Tonight' mockumentary...",
                        Content =
                            "THE DARK KNIGHT - 2-Disc Special Edition\n\nDisc 1: The Feature Film\nRuntime: 152 minutes\nFormat: Widescreen (2.40:1)\nAudio: English 5.1 Surround Sound, French, Spanish\nSubtitles: English, French, Spanish\n\nBatman raises the stakes in his war on crime. With the help of Lt. Jim Gordon and Harvey Dent, Batman sets out to dismantle the remaining criminal organizations that plague the streets. The partnership proves to be effective, but they soon find themselves prey to a reign of chaos unleashed by a rising criminal mastermind known to the terrified citizens of Gotham as the Joker.\n\nStarring:\n- Christian Bale as Bruce Wayne/Batman\n- Heath Ledger as The Joker (Academy Award Winner - Best Supporting Actor)\n- Aaron Eckhart as Harvey Dent/Two-Face\n- Michael Caine as Alfred Pennyworth\n- Maggie Gyllenhaal as Rachel Dawes\n- Gary Oldman as Lt. James Gordon\n- Morgan Freeman as Lucius Fox\n\nDirected by Christopher Nolan\n\nDisc 2: Special Features\n\n'Gotham Tonight' (42 minutes)\nA six-part mockumentary news program set in Gotham City, featuring interviews with key characters and exploring the city's reaction to Batman and the Joker's emergence.\n\n'Focus Points' - Interactive Feature\nAccess behind-the-scenes content while watching the film, including:\n- The psychology of the Joker\n- Practical effects vs. CGI\n- Filming locations in Chicago\n- Heath Ledger's preparation for the role\n\nCharacter Profiles (25 minutes):\n- 'The Joker: Concept to Character' - Heath Ledger's transformative performance\n- 'Harvey Dent: The White Knight's Fall' - Aaron Eckhart discusses Two-Face\n- 'Batman: Beyond the Mask' - Christian Bale on evolving the character\n\n'The Making of The Dark Knight' (30 minutes)\nComprehensive behind-the-scenes documentary covering:\n- Pre-production and screenplay development\n- Casting Heath Ledger as the Joker\n- Filming the truck flip sequence\n- Creating Two-Face's makeup effects\n- The IMAX experience\n\nDeleted Scenes (8 minutes):\n- Extended bank robbery opening\n- Additional Joker interrogation footage\n- Alternate Harvey Dent press conference\n- Extended hospital explosion sequence\n\n'The Dark Knight IMAX Experience' (20 minutes)\nExploring Christopher Nolan's groundbreaking use of IMAX cameras for key action sequences, including the opening bank heist and the truck chase.\n\n'Batman Tech' (12 minutes)\nA look at the real-world inspiration behind Batman's gadgets and vehicles, featuring interviews with military and technology experts.\n\n'Batman Unmasked: The Psychology of the Dark Knight' (15 minutes)\nPsychologists and comic book experts analyze the psychological aspects of Batman's war on crime and the Joker's chaos philosophy.\n\nProduction Gallery:\n- Concept art and storyboards\n- Behind-the-scenes photography\n- Costume and makeup tests\n- Location scouting photos\n\nMarketing Archive:\n- Theatrical trailers (4)\n- TV spots and international promos (15)\n- Viral marketing content from 'Why So Serious?' campaign\n- Poster gallery\n\nAudio Commentary Options:\n- Director Christopher Nolan and cinematographer Wally Pfister\n- Production team commentary with producers and key crew members\n\nEaster Eggs:\n- Hidden Joker playing cards in menu navigation\n- Alternate menu designs based on chaos vs. order theme\n- Secret Harvey Dent campaign videos\n\nBonus Features:\n- Digital copy download code\n- Exclusive comic book: 'Gotham Knight' preview\n- Limited edition collector's booklet (16 pages)\n\nTechnical Specifications:\nAspect Ratio: 2.40:1 (with IMAX sequences in 1.43:1)\nAudio: English 5.1 Dolby Digital, French 5.1, Spanish 5.1\nSubtitles: English SDH, French, Spanish\nRegion: Region 1 (US/Canada)\n\nRated PG-13 for intense sequences of violence and some menace.\n\nTotal Runtime (with special features): Over 6 hours\n\nCopyright 2008 Warner Bros. Entertainment Inc. All rights reserved.",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 10,
                        Title = "Inception",
                        Author = "Warner Bros. Pictures",
                        ISBN = "BLU-2010-INCEPTION",
                        PublicationYear = 2010,
                        Genre = "Sci-Fi/Thriller",
                        Publisher = "Warner Home Video",
                        PageCount = 15,
                        Type = ResourceType.Media,
                        IsAvailable = true,
                        Description =
                            "Dom Cobb is a skilled thief who specializes in extraction - stealing secrets from deep within the subconscious during the dream state. Now he must perform the impossible: inception. Blu-ray Collector's Edition with extensive bonus content.",
                        ContentPreview =
                            "Blu-ray Feature Film (148 minutes) in stunning 1080p HD\nExtensive special features exploring the science of dreams and practical effects...",
                        Content =
                            "INCEPTION - Blu-ray Collector's Edition\n\nMain Feature:\nRuntime: 148 minutes\nFormat: 1080p High Definition (2.40:1)\nAudio: English DTS-HD Master Audio 5.1, French DTS 5.1, Spanish DTS 5.1\nSubtitles: English SDH, French, Spanish, Portuguese\n\nDom Cobb is a skilled thief, the absolute best in the dangerous art of extraction, stealing valuable secrets from deep within the subconscious during the dream state, when the mind is at its most vulnerable. Cobb's rare ability has made him a coveted player in this treacherous new world of corporate espionage, but it has also made him an international fugitive and cost him everything he has ever loved.\n\nNow Cobb is being offered a chance at redemption. One last job could give him his life back but only if he can accomplish the impossible - inception. Instead of the perfect heist, Cobb and his team of specialists have to pull off the reverse: their task is not to steal an idea but to plant one.\n\nStarring:\n- Leonardo DiCaprio as Dom Cobb\n- Ken Watanabe as Saito\n- Joseph Gordon-Levitt as Arthur\n- Marion Cotillard as Mal\n- Ellen Page as Ariadne\n- Tom Hardy as Eames\n- Cillian Murphy as Robert Fischer\n- Michael Caine as Professor Miles\n\nWritten and Directed by Christopher Nolan\n\nSpecial Features:\n\n'Extraction Mode' - Enhanced Viewing Experience\nWatch the film with picture-in-picture behind-the-scenes content, including:\n- Cast and crew interviews\n- Production photos and concept art\n- Technical breakdowns of complex sequences\n- Dream level navigation guide\n\n'Dreams: Cinema of the Subconscious' (44 minutes)\nA comprehensive documentary exploring:\n- The science of lucid dreaming\n- Christopher Nolan's inspiration and creative process\n- The film's complex narrative structure\n- Practical effects vs. digital enhancement\n\n'Inception: The Cobol Job' (14 minutes)\nAnimated prequel comic explaining the backstory of Dom Cobb's relationship with Cobol Engineering and the events leading up to the film.\n\n'5.1 Ideas: The Inception of Christopher Nolan' (35 minutes)\nIn-depth interview with director Christopher Nolan discussing:\n- The 10-year development process\n- Influences from classic heist films\n- Working with IMAX cameras\n- The film's ending and interpretation\n\nConcept and Production Galleries:\n- Pre-visualization sequences\n- Storyboard comparisons\n- Costume design evolution\n- Set construction time-lapse\n- Location scouting worldwide\n\n'Constructing Paradox: The Production Design' (22 minutes)\nProduction designer Guy Hendrix Dyas discusses creating the film's multiple dream worlds:\n- The hotel corridor fight sequence\n- Building Limbo's architecture\n- The spinning hallway construction\n- Creating impossible geometries\n\n'The Big Idea: Altering Architecture' (16 minutes)\nSpecial effects supervisor Paul Franklin explains the practical and digital effects:\n- The folding Paris sequence\n- Zero gravity fight choreography\n- The train in downtown Los Angeles\n- Limbo's crumbling buildings\n\n'Inception: 4 Levels of Dreams' Interactive Map\nNavigate through the film's complex dream structure with:\n- Character tracking across levels\n- Time dilation explanations\n- Kick synchronization breakdown\n- Totem significance guide\n\nDeleted and Extended Scenes (12 minutes):\n- Extended Mombasa chase sequence\n- Additional Mal backstory\n- Alternate limbo conversations\n- Extended fortress assault\n\nMarketing Archive:\n- Theatrical trailers (3)\n- TV spots (8)\n- International promotional content\n- Mind Crime viral videos\n- Poster gallery\n\nAudio Commentary:\n- Christopher Nolan (Director/Writer)\n- Wally Pfister (Cinematographer) \n- Lee Smith (Editor)\n- Guy Hendrix Dyas (Production Designer)\n\nTechnical Specifications:\nVideo: 1080p/AVC MPEG-4 (2.40:1)\nAudio: English DTS-HD Master Audio 5.1\nSubtitles: Multiple languages available\nRegion: Region A (Americas, East Asia)\nDiscs: 2 (1 Blu-ray, 1 DVD, 1 Digital Copy)\n\nPackaging:\n- Collectible SteelBook case\n- 40-page production booklet\n- Exclusive concept art postcards (8)\n- Digital copy ultraviolet code\n\nBonus Digital Features:\n- BD-Live exclusive content\n- Warner Bros. movie trailers\n- Enhanced UltraViolet experience\n\nRated PG-13 for sequences of violence and action throughout.\n\nTotal Special Features Runtime: Over 4 hours\n\nCopyright 2010 Warner Bros. Entertainment Inc. All rights reserved.",
                        Rating = 4.9m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 18,
                        Title = "The Matrix",
                        Author = "Warner Bros. Pictures",
                        ISBN = "DVD-1999-MATRIX-001",
                        PublicationYear = 1999,
                        Genre = "Sci-Fi/Action",
                        Publisher = "Warner Home Video",
                        PageCount = 10,
                        Type = ResourceType.Media,
                        IsAvailable = true,
                        Description =
                            "Neo discovers that reality as he knows it is actually a computer simulation. He must choose between the blissful ignorance of illusion and the painful truth of reality. Special Edition with groundbreaking behind-the-scenes content.",
                        ContentPreview =
                            "Feature Film (136 minutes) + Extensive special features exploring groundbreaking visual effects and philosophical themes...",
                        Content =
                            "THE MATRIX - Special Edition DVD\n\nFeature Film:\nRuntime: 136 minutes\nFormat: Widescreen (2.35:1)\nAudio: English 5.1 Dolby Digital, French, Spanish\nSubtitles: English, French, Spanish\n\nWhen a beautiful stranger leads computer hacker Neo to a forbidding underworld, he discovers the shocking truth - the life he knows is the elaborate deception of an evil cyber-intelligence.\n\nStarring:\n- Keanu Reeves as Neo/Thomas Anderson\n- Laurence Fishburne as Morpheus\n- Carrie-Anne Moss as Trinity\n- Hugo Weaving as Agent Smith\n- Joe Pantoliano as Cypher\n- Marcus Chong as Tank\n\nDirected by The Wachowski Brothers\n\nSpecial Features:\n\n'The Philosophy of The Matrix' (28 minutes)\nScholars and philosophers discuss the film's exploration of:\n- Plato's Cave allegory\n- Descartes' reality questioning\n- Buddhist concepts of illusion (Maya)\n- Gnostic religious themes\n- Simulation theory and modern philosophy\n\n'Making The Matrix' (26 minutes)\nBehind-the-scenes documentary covering:\n- Conceptual development and storyboarding\n- Groundbreaking 'bullet time' effect creation\n- Wire work and martial arts training\n- Set construction for Nebuchadnezzar ship\n- Creating the green digital rain effect\n\n'The Burly Man Chronicles' (85 minutes)\nExclusive documentary following the production from start to finish:\n- Pre-production planning and design\n- Casting process and actor preparation\n- Daily filming challenges and solutions\n- Post-production visual effects work\n- Marketing and release strategy\n\n'Follow the White Rabbit' - Enhanced Viewing Mode\nInteractive feature allowing viewers to access behind-the-scenes content during specific scenes:\n- Storyboard comparisons\n- Alternate takes and angles\n- Technical explanations\n- Cast and crew commentary\n\nDeleted Scenes (15 minutes):\n- Extended opening Trinity chase\n- Additional Nebuchadnezzar crew interactions\n- Alternate Agent interrogation sequences\n- Extended subway fight with Agent Smith\n\n'The Matrix: What Is Bullet Time?' (8 minutes)\nTechnical breakdown of the revolutionary visual effect:\n- Camera rig construction\n- Filming process and timing\n- Digital compositing techniques\n- Impact on future filmmaking\n\nConcept Art Gallery:\n- Character design evolution\n- Set and costume concepts\n- Storyboard artwork\n- The Matrix code visual development\n\nMusic Videos:\n- 'Wake Up' by Rage Against the Machine\n- 'Rock Is Dead' by Marilyn Manson\n- Behind-the-scenes music video footage\n\nWeb Links:\n- Official Matrix website access\n- Cast and crew filmographies\n- Technical specifications\n- Awards and recognition\n\nEaster Eggs:\n- Hidden philosophical quotes in menu navigation\n- Matrix code screensavers\n- Red pill / blue pill menu choices\n- Agent Smith virus simulation\n\nAudio Commentary:\n- The Wachowski Brothers (Directors/Writers)\n- Cast commentary with Keanu Reeves and Carrie-Anne Moss\n- Technical commentary with visual effects supervisors\n\nMarketing Archive:\n- Original theatrical trailers (3)\n- TV spots and promotional content (12)\n- International advertising materials\n- Poster collection\n\nTechnical Specifications:\nAspect Ratio: 2.35:1 Anamorphic Widescreen\nAudio: English 5.1 Dolby Digital, French 2.0, Spanish 2.0\nSubtitles: English, French, Spanish, Portuguese\nRegion: Region 1 (US/Canada)\nLayers: Dual Layer\n\nSpecial Packaging:\n- Collectible keepcase with holographic slipcover\n- 16-page booklet with production notes\n- Character profile cards (4)\n\nSystem Requirements:\nDVD player or computer DVD drive\nDolby Digital compatible sound system for full audio experience\n\nRated R for sci-fi violence and brief language.\n\nTotal Runtime (with special features): Over 5 hours\n\nCopyright 1999 Warner Bros. Pictures. All rights reserved.",
                        Rating = 4.7m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 19,
                        Title = "Interstellar",
                        Author = "Paramount Pictures",
                        ISBN = "BLU-2014-INTERSTELLAR",
                        PublicationYear = 2014,
                        Genre = "Sci-Fi/Drama",
                        Publisher = "Paramount Home Entertainment",
                        PageCount = 12,
                        Type = ResourceType.Media,
                        IsAvailable = true,
                        Description =
                            "In Earth's future, a global crop blight and second Dust Bowl are slowly rendering the planet uninhabitable. Cooper, an ex-NASA pilot turned farmer, is tasked to pilot a spacecraft to find mankind a new home. Limited Edition Blu-ray with extensive scientific bonus content.",
                        ContentPreview =
                            "Feature Film (169 minutes) in stunning 4K quality + Scientific documentaries exploring real space science...",
                        Content =
                            "INTERSTELLAR - Limited Edition Blu-ray\n\nMain Feature:\nRuntime: 169 minutes\nFormat: 1080p High Definition (2.40:1)\nAudio: English DTS-HD Master Audio 5.1, French DTS 5.1, Spanish DTS 5.1\nSubtitles: English SDH, French, Spanish, Portuguese, Mandarin\n\nIn Earth's future, a global crop blight and second Dust Bowl are slowly rendering the planet uninhabitable. Professor Brand, a brilliant NASA physicist, is working on plans to save mankind by transporting Earth's population to a new home via a wormhole. But first, Brand must send former NASA pilot Cooper and a team of researchers through the wormhole and across the galaxy to find out which of three planets could be mankind's new home.\n\nStarring:\n- Matthew McConaughey as Cooper\n- Anne Hathaway as Dr. Amelia Brand\n- Jessica Chastain as Murph (adult)\n- Michael Caine as Professor Brand\n- Casey Affleck as Tom Cooper\n- Wes Bentley as Doyle\n- Matt Damon as Dr. Mann\n- Mackenzie Foy as Murph (child)\n\nDirected by Christopher Nolan\n\nSpecial Features:\n\n'The Science of Interstellar' (51 minutes)\nPhysicist Kip Thorne, who served as executive producer and scientific consultant, explains:\n- Black hole physics and visualization\n- Time dilation and relativity theory\n- Wormhole theoretical construction\n- The accuracy of Gargantua's depiction\n- Fifth-dimensional space concepts\n\n'Plotting an Interstellar Journey' (24 minutes)\nChristopher Nolan discusses:\n- The film's scientific foundation\n- Collaboration with Kip Thorne\n- Balancing science with storytelling\n- Practical effects vs. digital enhancement\n- IMAX filming techniques\n\n'Life on Cooper's Farm' (12 minutes)\nExploring the film's Earth-based sequences:\n- Creating the dust bowl environment\n- Corn field cultivation for filming\n- Production design of Cooper's farmhouse\n- Capturing the agricultural crisis\n\n'The Dust' (8 minutes)\nSpecial effects breakdown of the omnipresent dust storms:\n- Practical dust effect creation\n- Digital enhancement techniques\n- Safety protocols during filming\n- Environmental storytelling through dust\n\n'TARS and CASE: Designing the Robots' (17 minutes)\nThe creation of the film's unique robot characters:\n- Physical puppet construction\n- Movement and puppeteer coordination\n- Voice recording and processing\n- Character personality development\n\n'Cosmic Sounds: The Music of Interstellar' (15 minutes)\nComposer Hans Zimmer discusses:\n- Organ-based musical themes\n- Emotional resonance in space\n- Recording in unique acoustic environments\n- The 'No Time for Caution' docking sequence score\n\n'The Space Suits' (6 minutes)\nCostume design and practical considerations:\n- NASA consultation for authenticity\n- Actor mobility and comfort\n- Helmet communication systems\n- Zero-G movement simulation\n\n'Shooting in Iceland: Miller's Planet' (14 minutes)\nLocation filming on the ice planet:\n- Scouting remote Icelandic locations\n- Extreme weather filming challenges\n- Creating otherworldly landscapes\n- Practical water effects\n\n'The Endurance: Spacecraft Design' (11 minutes)\nProduction design of the main spacecraft:\n- Modular construction philosophy\n- Rotating sections and artificial gravity\n- Set construction and camera movement\n- Interior lighting and atmosphere\n\n'Visualizing the Wormhole and Black Hole' (19 minutes)\nGroundbreaking visual effects creation:\n- Scientific accuracy in rendering\n- New software development for Gargantua\n- Light bending and gravitational lensing\n- The accretion disk's realistic appearance\n\nDeleted and Extended Scenes (23 minutes):\n- Extended Cooper family breakfast\n- Additional Earth's dying environment footage\n- Longer Dr. Mann confrontation\n- Alternate tesseract sequence takes\n\n'Theoretical Astrophysics: Kip Thorne's Insights' (31 minutes)\nAdvanced scientific discussion covering:\n- Warped spacetime visualization\n- Closed timelike curves\n- Higher dimensional physics\n- The intersection of science and cinema\n\nAudio Commentary:\n- Christopher Nolan (Director/Writer)\n- Jonathan Nolan (Writer)\n- Kip Thorne (Executive Producer/Scientific Consultant)\n- Hoyte van Hoytema (Cinematographer)\n\nIMAX Enhanced Version:\n- Select sequences presented in expanded aspect ratio\n- Enhanced detail and scope for home viewing\n- Director's preferred presentation format\n\nMarketing Archive:\n- Theatrical trailers (4)\n- TV spots and promotional content (15)\n- International marketing materials\n- Scientific promotional videos\n- Poster gallery\n\nTechnical Specifications:\nVideo: 1080p/AVC MPEG-4 (2.40:1)\nAudio: English DTS-HD Master Audio 5.1\nSubtitles: Multiple languages available\nRegion: Region A (Americas, East Asia)\nDiscs: 3 (2 Blu-ray, 1 DVD)\n\nPackaging:\n- Premium digipak case with magnetic closure\n- 64-page companion booklet with scientific essays\n- Exclusive concept art prints (6)\n- Digital HD UltraViolet copy included\n\nBonus Digital Content:\n- Interactive periodic table\n- NASA partnership content\n- Scientific accuracy comparisons\n- Real space mission footage\n\nRated PG-13 for some intense perilous action and brief strong language.\n\nTotal Special Features Runtime: Over 4 hours\n\nCopyright 2014 Paramount Pictures Corporation. All rights reserved.",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    // more books - classic literature
                    new LibraryResource
                    {
                        Id = 11,
                        Title = "The Hobbit's Great Adventure",
                        Author = "R.R. Tolkien",
                        ISBN = "978-5-555-11111-1",
                        PublicationYear = 1937,
                        Genre = "Fantasy",
                        Publisher = "Fantasy Classics",
                        PageCount = 300,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "A classic tale of adventure featuring a reluctant hero's journey through magical lands.",
                        ContentPreview =
                            "In a hole in the ground there lived a hobbit. Not a nasty, dirty, wet hole filled with worms and oozing smells, but a comfortable hobbit-hole with round doors and windows...",
                        Content =
                            "Chapter 1: An Unexpected Journey\n\nBilbo Baggins was a hobbit who enjoyed the quiet life. His comfortable home, good food, and predictable routine suited him perfectly. But on one particular morning, his world was about to change forever.\n\nA knock at the door interrupted his second breakfast. Standing on his doorstep was an old man with a pointed hat and a long grey beard.\n\n'Good morning!' said the stranger cheerfully.\n\n'What do you mean?' asked Bilbo, quite flustered. 'Do you wish me a good morning, or mean that it is a good morning whether I want it or not, or that you feel good this morning, or that it is a morning to be good on?'\n\nThe old man chuckled. 'All of them at once, I suppose. I am looking for someone to share in an adventure that I am arranging, and it's very difficult to find anyone suitable.'\n\n'Adventures?' Bilbo spluttered. 'Nasty disturbing uncomfortable things! Make you late for dinner! I don't see what anybody sees in them.'\n\nBut despite his protests, Bilbo found himself drawn into a tale of distant mountains, sleeping dragons, and lost treasure. Before he knew it, he was signing a contract to serve as a 'burglar' for a company of dwarves on a quest to reclaim their homeland.\n\nAs he packed his bags with trembling hands, Bilbo wondered what he had gotten himself into. Little did he know that this journey would transform him from a timid homebody into one of the bravest adventurers in all of Middle-earth.",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 12,
                        Title = "Pride and Modern Prejudice",
                        Author = "Jane Classic",
                        ISBN = "978-6-666-22222-2",
                        PublicationYear = 1813,
                        Genre = "Romance",
                        Publisher = "Classic Literature Ltd",
                        PageCount = 280,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "A timeless story of love, misunderstanding, and social expectations in Regency England.",
                        ContentPreview =
                            "It is a truth universally acknowledged that a single man in possession of a good fortune must be in want of a wife. However little known the feelings or views of such a man may be...",
                        Content =
                            "Chapter 1: First Impressions\n\nIt is a truth universally acknowledged that a single man in possession of a good fortune must be in want of a wife. However little known the feelings or views of such a man may be on his first entering a neighbourhood, this truth is so well fixed in the minds of the surrounding families, that he is considered as the rightful property of some one or other of their daughters.\n\nElizabeth Bennet had heard this maxim many times from her mother, but she found it rather tiresome. At twenty years old, she was more interested in books and long walks than in the marriage market that seemed to consume the thoughts of everyone around her.\n\n'My dear Mr. Bennet,' said his lady to him one day, 'have you heard that Netherfield Park is let at last?'\n\nMr. Bennet replied that he had not.\n\n'But it is,' returned she; 'for Mrs. Long has just been here, and she told me all about it. A young man of large fortune from the north of England has taken it. His name is Bingley, and he is single!'\n\nElizabeth rolled her eyes at her mother's obvious excitement. Another wealthy gentleman to be paraded before her and her sisters like merchandise at market. She much preferred the company of her books to the artificial conversations that such social situations demanded.\n\nLittle did she know that this Mr. Bingley would bring with him a friend whose pride would clash spectacularly with her own prejudices, setting in motion a series of misunderstandings that would challenge everything she thought she knew about love and human nature.",
                        Rating = 4.7m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 13,
                        Title = "1984: A Dystopian Future",
                        Author = "George Orwell",
                        ISBN = "978-7-777-33333-3",
                        PublicationYear = 1949,
                        Genre = "Dystopian Fiction",
                        Publisher = "Political Press",
                        PageCount = 320,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "A chilling vision of a totalitarian future where freedom is forbidden and truth is manipulated.",
                        ContentPreview =
                            "It was a bright cold day in April, and the clocks were striking thirteen. Winston Smith, his chin nuzzled into his breast in an effort to escape the vile wind, slipped quickly through the glass doors...",
                        Content =
                            "Chapter 1: The Ministry of Truth\n\nIt was a bright cold day in April, and the clocks were striking thirteen. Winston Smith, his chin nuzzled into his breast in an effort to escape the vile wind, slipped quickly through the glass doors of Victory Mansions, though not quickly enough to prevent a swirl of gritty dust from entering along with him.\n\nThe hallway smelt of boiled cabbage and old rag mats. At one end of it a coloured poster, too large for indoor display, had been tacked to the wall. It depicted simply an enormous face, more than a metre wide: the face of a man of about forty-five, with a heavy black moustache and ruggedly handsome features.\n\nWinston made for the stairs. It was no use trying the lift. Even at the best of times it was seldom working, and at present the electric current was cut off during daylight hours. It was part of the economy drive in preparation for Hate Week.\n\nThe flat was seven flights up, and Winston, who was thirty-nine and had a varicose ulcer above his right ankle, went slowly, resting several times on the way. On each landing, opposite the lift shaft, the poster with the enormous face gazed from the wall. It was one of those pictures which are so contrived that the eyes follow you about when you move.\n\n'BIG BROTHER IS WATCHING YOU,' the caption beneath it ran.\n\nInside the flat a fruity voice was reading out a list of figures which had something to do with the production of pig-iron. The voice came from an oblong metal plaque like a dulled mirror which formed part of the surface of the right-hand wall.\n\nWinston turned a switch and the voice sank somewhat, though the words were still distinguishable. The instrument (the telescreen, it was called) could be dimmed, but there was no way of shutting it off completely.",
                        Rating = 4.6m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 14,
                        Title = "To Kill a Mockingbird's Legacy",
                        Author = "Harper Lee",
                        ISBN = "978-8-888-44444-4",
                        PublicationYear = 1960,
                        Genre = "Social Fiction",
                        Publisher = "Justice Publishing",
                        PageCount = 290,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "A powerful story about justice, morality, and growing up in the American South during the 1930s.",
                        ContentPreview =
                            "When I was almost six and Jem was nearly ten, our summertime boundaries were Mrs. Henry Lafayette Dubose's house two doors to the north of us, and the Radley Place three doors to the south...",
                        Content =
                            "Chapter 1: Maycomb County\n\nWhen I was almost six and Jem was nearly ten, our summertime boundaries were Mrs. Henry Lafayette Dubose's house two doors to the north of us, and the Radley Place three doors to the south. We were never tempted to break them, for the Radley Place was inhabited by an unknown entity the mere description of whom was enough to make us behave for days on end.\n\nMaycomb was an old town, but it was a tired old town when I first knew it. In rainy weather the streets turned to red slop; grass grew on the sidewalks, the courthouse sagged in the square. Somehow, it was hotter then: a black dog suffered on a summer's day; bony mules hitched to Hoover carts flicked flies in the sweltering shade of the live oaks on the square.\n\nMen's stiff collars wilted by nine in the morning. Ladies bathed before noon, after their three-o'clock naps, and by nightfall were like soft teacakes with frostings of sweat and sweet talcum.\n\nPeople moved slowly then. They ambled across the square, shuffled in and out of the stores around it, took their time about everything. A day was twenty-four hours long but seemed longer. There was no hurry, for there was nowhere to go, nothing to buy and no money to buy it with, nothing to see outside the boundaries of Maycomb County.\n\nBut it was a time of vague optimism for some of the people: Maycomb County had recently been told that it had nothing to fear but fear itself.",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 15,
                        Title = "The Great Gatsby's Era",
                        Author = "F. Scott Fitzgerald",
                        ISBN = "978-9-999-55555-5",
                        PublicationYear = 1925,
                        Genre = "American Literature",
                        Publisher = "Jazz Age Books",
                        PageCount = 250,
                        Type = ResourceType.Book,
                        IsAvailable = true,
                        Description =
                            "A classic tale of the American Dream set during the Jazz Age of the 1920s.",
                        ContentPreview =
                            "In my younger and more vulnerable years my father gave me some advice that I've carried with me ever since. 'Whenever you feel like criticizing anyone,' he told me, 'just remember that all the people in this world haven't had the advantages that you've had.'",
                        Content =
                            "Chapter 1: West Egg\n\nIn my younger and more vulnerable years my father gave me some advice that I've carried with me ever since. 'Whenever you feel like criticizing anyone,' he told me, 'just remember that all the people in this world haven't had the advantages that you've had.'\n\nHe didn't say any more, but we've always been unusually communicative in a reserved way, and I understood that he meant a great deal more than that. In consequence, I'm inclined to reserve all judgments, a habit that has opened up many curious natures to me and also made me the victim of not a few veteran bores.\n\nAnd, after boasting this way of my tolerance, I come to the admission that it has a limit. Conduct may be founded on the hard rock or the wet marshes, but after a certain point I don't care what it's founded on. When I came back from the East last autumn I felt that I wanted the world to be in uniform and at a sort of moral attention forever; I wanted no more riotous excursions with privileged glimpses into the human heart.\n\nOnly Gatsby, the man who gives his name to this book, was exempt from my reaction—Gatsby, who represented everything for which I have an unaffected scorn. If personality is an unbroken series of successful gestures, then there was something gorgeous about him, some heightened sensitivity to the promises of life.",
                        Rating = 4.7m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    // more journals - computer science and medical
                    new LibraryResource
                    {
                        Id = 16,
                        Title = "Journal of Computer Science Research",
                        Author = "Dr. Alan Turing",
                        ISBN = "ISSN-3333-9999",
                        PublicationYear = 2024,
                        Genre = "Computer Science",
                        Publisher = "Tech Academic Press",
                        PageCount = 52,
                        Type = ResourceType.Journal,
                        IsAvailable = true,
                        Description =
                            "Leading publication featuring cutting-edge research in artificial intelligence, machine learning, and computational theory.",
                        ContentPreview =
                            "This edition explores breakthrough developments in quantum computing and its applications to cryptography and optimization problems...",
                        Content =
                            "Volume 45, Issue 2 - April 2024\n\nEditor's Note: The Future of Quantum Computing\nBy Dr. Alan Turing\n\nThis edition explores breakthrough developments in quantum computing and its applications to cryptography and optimization problems. Recent advances in quantum error correction have brought us closer to practical quantum computers that could revolutionize computing as we know it.\n\nFeatured Research Articles:\n\n1. 'Quantum Machine Learning: Beyond Classical Limitations'\nBy Dr. Marie Curie, MIT\n\nOur research demonstrates that quantum machine learning algorithms can achieve exponential speedups over classical methods for certain types of pattern recognition problems. We present a novel quantum neural network architecture that successfully classifies complex datasets with unprecedented accuracy.\n\nKey findings:\n- Quantum neural networks show 300% improvement in processing speed\n- Error rates reduced by 85% compared to traditional algorithms\n- Scalable architecture supports datasets with millions of parameters\n\n2. 'Blockchain Security in the Quantum Era'\nBy Dr. Satoshi Nakamoto, Stanford University\n\nAs quantum computers threaten current cryptographic systems, we must develop quantum-resistant blockchain technologies. This paper presents a new consensus mechanism that remains secure even against quantum attacks.\n\nOur proposed solution:\n- Post-quantum cryptographic signatures\n- Lattice-based hash functions\n- Quantum-resistant proof-of-work algorithms\n\n3. 'AI Ethics: Designing Responsible Machine Learning Systems'\nBy Dr. Ada Lovelace, Oxford University\n\nThe rapid advancement of AI technology raises critical ethical questions about bias, privacy, and accountability. This study examines current approaches to ethical AI development and proposes new frameworks for responsible innovation.\n\nEthical considerations:\n- Algorithmic fairness across diverse populations\n- Privacy-preserving machine learning techniques\n- Transparent decision-making processes\n- Human oversight and control mechanisms\n\nTechnical Notes:\n\n'Optimizing Database Performance with AI'\nBy Dr. Edgar Codd, IBM Research\n\nModern databases can leverage machine learning to automatically optimize query performance and resource allocation. Our experimental system shows 40% improvement in query response times.\n\nBook Reviews:\n\n'Artificial Intelligence: A Modern Approach (5th Edition)'\nReviewed by Dr. John McCarthy\n\nThe latest edition incorporates recent developments in deep learning and reinforcement learning. Essential reading for anyone serious about AI research.\n\nUpcoming Conferences:\n\n- International Conference on Machine Learning (ICML 2024) - July 15-18, Vienna\n- Conference on Neural Information Processing Systems (NeurIPS 2024) - December 10-16, Vancouver\n- Quantum Computing Symposium - September 5-7, Tokyo\n\nCall for Papers:\n\nSpecial issue on 'Explainable AI in Healthcare' - Submission deadline: June 30, 2024\nWe seek original research on developing interpretable machine learning models for medical diagnosis and treatment recommendation.\n\nSubscription Information:\nAnnual subscription: $120 (Digital) / $180 (Print + Digital)\nStudent rate: $60 with valid academic ID\nInstitutional rate: $500\n\nISSN: 3333-9999\nPublished bi-monthly by Tech Academic Press",
                        Rating = 4.5m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    },
                    new LibraryResource
                    {
                        Id = 17,
                        Title = "International Medical Research Quarterly",
                        Author = "Dr. Florence Nightingale",
                        ISBN = "ISSN-4444-7777",
                        PublicationYear = 2024,
                        Genre = "Medical Science",
                        Publisher = "Global Health Publications",
                        PageCount = 48,
                        Type = ResourceType.Journal,
                        IsAvailable = true,
                        Description =
                            "Prestigious medical journal featuring groundbreaking research in clinical medicine, public health, and biomedical sciences.",
                        ContentPreview =
                            "This quarter's focus examines revolutionary gene therapy treatments and their potential to cure previously incurable diseases...",
                        Content =
                            "Volume 78, Issue 1 - Q1 2024\n\nEditorial: Gene Therapy - The Next Medical Revolution\nBy Dr. Florence Nightingale, Editor-in-Chief\n\nThis quarter's focus examines revolutionary gene therapy treatments and their potential to cure previously incurable diseases. Recent clinical trials have shown remarkable success in treating genetic disorders, cancer, and even some viral infections.\n\nLead Articles:\n\n1. 'CRISPR-Cas9 Treatment for Sickle Cell Disease: 12-Month Follow-up'\nBy Dr. Jennifer Doudna, University of California\n\nOur landmark clinical trial followed 50 patients with severe sickle cell disease who received CRISPR-based gene editing treatment. Results show complete remission in 94% of patients with no significant adverse effects.\n\nPatient outcomes:\n- Zero pain crises reported in 47 of 50 patients\n- Hemoglobin levels normalized within 3 months\n- No blood transfusions required post-treatment\n- Quality of life scores improved by 340%\n\n2. 'Immunotherapy Breakthrough: Training T-Cells to Fight Pancreatic Cancer'\nBy Dr. James Allison, MD Anderson Cancer Center\n\nPancreatic cancer has long been considered one of the most challenging malignancies to treat. Our new CAR-T cell therapy shows unprecedented success rates in advanced-stage patients.\n\nClinical trial results:\n- 67% of patients showed tumor shrinkage\n- Median survival increased from 6 to 18 months\n- 23% achieved complete remission\n- Treatment well-tolerated with manageable side effects\n\n3. 'Global Impact of Malaria Vaccine Rollout in Sub-Saharan Africa'\nBy Dr. Anthony Fauci, WHO Collaborative Team\n\nThe widespread deployment of the new malaria vaccine has dramatically reduced infection rates across 15 African nations. This comprehensive analysis covers implementation challenges and remarkable health outcomes.\n\nPublic health impact:\n- 78% reduction in malaria cases among vaccinated children\n- Hospital admissions decreased by 65%\n- Childhood mortality from malaria reduced by 82%\n- Cost-effective implementation model developed\n\nCase Studies:\n\n'Telemedicine in Rural Healthcare: Lessons from the Pandemic'\nBy Dr. Atul Gawande, Harvard Medical School\n\nCOVID-19 accelerated telemedicine adoption worldwide. This analysis examines which practices should be permanently integrated into healthcare delivery systems.\n\nResearch Notes:\n\n'AI-Assisted Radiology: Improving Diagnostic Accuracy'\nBy Dr. Geoffrey Hinton, Google Health\n\nMachine learning algorithms now outperform human radiologists in detecting certain types of cancer from medical imaging. We explore the implications for clinical practice.\n\nPharmacology Update:\n\n'New Alzheimer's Drug Shows Promise in Phase III Trials'\nBy Dr. Dale Schenk, Biogen Research\n\nAducanumab demonstrates significant cognitive improvement in early-stage Alzheimer's patients. FDA approval expected pending safety review.\n\nGlobal Health Perspective:\n\n'Addressing Healthcare Inequality: A Systems Approach'\nBy Dr. Paul Farmer, Partners in Health\n\nHealthcare disparities persist worldwide. This comprehensive framework outlines strategies for delivering quality care to underserved populations.\n\nUpcoming Medical Conferences:\n\n- World Health Assembly - May 22-30, Geneva\n- American Medical Association Annual Meeting - June 8-12, Chicago\n- International Conference on Global Health - August 15-17, London\n\nGrant Opportunities:\n\nNational Institutes of Health seeks applications for precision medicine research - Deadline: May 15, 2024\nGates Foundation announces $50M initiative for infectious disease prevention - Applications open April 1\n\nSubscription Information:\nAnnual subscription: $200 (Digital) / $300 (Print + Digital)\nResident/Student rate: $75 with verification\nLibrary institutional rate: $800\n\nISSN: 4444-7777\nPublished quarterly by Global Health Publications",
                        Rating = 4.8m,
                        CreatedDate = DateTime.Now,
                        LastModified = DateTime.Now,
                    }
                );

            // seed borrow records with realistic sample data
            modelBuilder
                .Entity<BorrowRecord>()
                .HasData(
                    // alex's borrowing history - mix of returned and current
                    new BorrowRecord
                    {
                        Id = 1,
                        ResourceId = 1, // The Young Wizard's Journey
                        BorrowerName = "alex",
                        BorrowDate = new DateTime(2025, 7, 15),
                        DueDate = new DateTime(2025, 7, 29),
                        ReturnDate = new DateTime(2025, 7, 28),
                        IsReturned = true,
                    },
                    new BorrowRecord
                    {
                        Id = 2,
                        ResourceId = 8, // The Avengers
                        BorrowerName = "alex",
                        BorrowDate = new DateTime(2025, 7, 20),
                        DueDate = new DateTime(2025, 8, 3),
                        ReturnDate = new DateTime(2025, 8, 1),
                        IsReturned = true,
                    },
                    new BorrowRecord
                    {
                        Id = 3,
                        ResourceId = 3, // Magical Creatures Encyclopedia
                        BorrowerName = "alex",
                        BorrowDate = new DateTime(2025, 7, 25),
                        DueDate = new DateTime(2025, 8, 8),
                        ReturnDate = null,
                        IsReturned = false, // currently borrowed
                    },
                    // ashu's borrowing history - includes one overdue
                    new BorrowRecord
                    {
                        Id = 4,
                        ResourceId = 2, // Heroes United
                        BorrowerName = "ashu",
                        BorrowDate = new DateTime(2025, 7, 10),
                        DueDate = new DateTime(2025, 7, 24),
                        ReturnDate = new DateTime(2025, 7, 23),
                        IsReturned = true,
                    },
                    new BorrowRecord
                    {
                        Id = 5,
                        ResourceId = 6, // Journal of Advanced Magical Theory
                        BorrowerName = "ashu",
                        BorrowDate = new DateTime(2025, 7, 18),
                        DueDate = new DateTime(2025, 8, 1),
                        ReturnDate = null,
                        IsReturned = false, // currently borrowed
                    },
                    new BorrowRecord
                    {
                        Id = 6,
                        ResourceId = 9, // The Dark Knight
                        BorrowerName = "ashu",
                        BorrowDate = new DateTime(2025, 7, 12),
                        DueDate = new DateTime(2025, 7, 26), // overdue
                        ReturnDate = null,
                        IsReturned = false, // overdue item
                    },
                    new BorrowRecord
                    {
                        Id = 7,
                        ResourceId = 13, // 1984: A Dystopian Future
                        BorrowerName = "ashu",
                        BorrowDate = new DateTime(2025, 7, 22),
                        DueDate = new DateTime(2025, 8, 5),
                        ReturnDate = new DateTime(2025, 8, 4),
                        IsReturned = true,
                    }
                );
        }
    }
}
