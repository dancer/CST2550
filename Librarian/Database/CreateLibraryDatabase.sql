-- =====================================================
-- Library Management System Database Creation Script
-- CST2550 RESET Coursework - Synchronized with Application Data
-- =====================================================

-- Drop database if exists (for clean recreation)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'LibraryDB')
BEGIN
    ALTER DATABASE LibraryDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE LibraryDB;
END
GO

-- Create fresh database
CREATE DATABASE LibraryDB;
GO

USE LibraryDB;
GO

-- =====================================================
-- TABLE CREATION
-- =====================================================

-- Main resources table matching LibraryResource model
CREATE TABLE Resources (
    Id int IDENTITY(1,1) PRIMARY KEY,
    Title nvarchar(255) NOT NULL,
    Author nvarchar(150) NOT NULL,
    ISBN nvarchar(50) NULL,
    PublicationYear int NOT NULL,
    Genre nvarchar(100) NOT NULL,
    Publisher nvarchar(200) NULL,
    PageCount int NULL,
    Language nvarchar(50) NULL DEFAULT 'English',
    Type int NOT NULL, -- 0=Book, 1=Journal, 2=Media
    IsAvailable bit NOT NULL DEFAULT 1,
    Description nvarchar(MAX) NULL,
    Content nvarchar(MAX) NULL,
    ContentPreview nvarchar(1000) NULL,
    CoverImagePath nvarchar(500) NULL,
    Rating decimal(3,2) NULL,
    CreatedDate datetime2 NOT NULL DEFAULT GETDATE(),
    LastModified datetime2 NOT NULL DEFAULT GETDATE()
);
GO

-- Borrow records table (matching EF model exactly)
CREATE TABLE BorrowRecords (
    Id int IDENTITY(1,1) PRIMARY KEY,
    ResourceId int NOT NULL,
    BorrowerName nvarchar(100) NOT NULL,
    BorrowDate datetime2 NOT NULL DEFAULT GETDATE(),
    DueDate datetime2 NOT NULL,
    ReturnDate datetime2 NULL,
    IsReturned bit NOT NULL DEFAULT 0,
    
    CONSTRAINT FK_BorrowRecords_Resource 
        FOREIGN KEY (ResourceId) REFERENCES Resources(Id) ON DELETE CASCADE
);
GO

-- Reading sessions table
CREATE TABLE ReadingSessions (
    Id int IDENTITY(1,1) PRIMARY KEY,
    ResourceId int NOT NULL,
    ReaderName nvarchar(150) NOT NULL,
    StartTime datetime2 NOT NULL DEFAULT GETDATE(),
    EndTime datetime2 NULL,
    CurrentPage int NOT NULL DEFAULT 1,
    TotalPages int NOT NULL,
    IsCompleted bit NOT NULL DEFAULT 0,
    BookmarkPosition int NULL,
    Notes nvarchar(MAX) NULL,
    
    CONSTRAINT FK_ReadingSessions_Resource 
        FOREIGN KEY (ResourceId) REFERENCES Resources(Id) ON DELETE CASCADE,
    CONSTRAINT CK_ReadingSessions_Pages CHECK (CurrentPage > 0 AND CurrentPage <= TotalPages)
);
GO

-- =====================================================
-- INDEXES FOR PERFORMANCE
-- =====================================================

CREATE NONCLUSTERED INDEX IX_Resources_Title ON Resources(Title);
CREATE NONCLUSTERED INDEX IX_Resources_Author ON Resources(Author);
CREATE NONCLUSTERED INDEX IX_Resources_Genre ON Resources(Genre);
CREATE NONCLUSTERED INDEX IX_Resources_Type ON Resources(Type);
CREATE NONCLUSTERED INDEX IX_Resources_ISBN ON Resources(ISBN);
CREATE NONCLUSTERED INDEX IX_Resources_PublicationYear ON Resources(PublicationYear);
CREATE NONCLUSTERED INDEX IX_Resources_IsAvailable ON Resources(IsAvailable);
CREATE NONCLUSTERED INDEX IX_Resources_Rating ON Resources(Rating);

CREATE NONCLUSTERED INDEX IX_BorrowRecords_ResourceId ON BorrowRecords(ResourceId);
CREATE NONCLUSTERED INDEX IX_BorrowRecords_BorrowerName ON BorrowRecords(BorrowerName);
CREATE NONCLUSTERED INDEX IX_BorrowRecords_DueDate ON BorrowRecords(DueDate);
CREATE NONCLUSTERED INDEX IX_BorrowRecords_IsReturned ON BorrowRecords(IsReturned);
CREATE NONCLUSTERED INDEX IX_BorrowRecords_BorrowDate ON BorrowRecords(BorrowDate);

CREATE NONCLUSTERED INDEX IX_ReadingSessions_Resource ON ReadingSessions(ResourceId);
CREATE NONCLUSTERED INDEX IX_ReadingSessions_Reader ON ReadingSessions(ReaderName);
CREATE NONCLUSTERED INDEX IX_ReadingSessions_StartTime ON ReadingSessions(StartTime);
GO

-- =====================================================
-- SAMPLE DATA - EXACTLY MATCHING APPLICATION
-- =====================================================

-- Insert all 19 resources from LibraryContext.cs
INSERT INTO Resources (Title, Author, ISBN, PublicationYear, Genre, Publisher, PageCount, Type, IsAvailable, Description, ContentPreview, Content, Rating, CreatedDate, LastModified) VALUES

-- === 10 BOOKS ===
('The Young Wizard''s Journey', 'A. Fantasy', '978-1-234-56789-0', 2021, 'Fantasy', 'Magic Books Ltd', 320, 0, 1,
'An original story about a young person discovering their magical abilities at a school for wizards.',
'Alex had always known they were different. Strange things happened around them - objects moved without being touched, lights flickered when they were upset, and sometimes they could swear they heard whispers from empty rooms. But nothing could have prepared them for the letter that arrived on their eleventh birthday...',
'Chapter 1: The Letter That Changed Everything

Alex had always known they were different. Strange things happened around them - objects moved without being touched, lights flickered when they were upset, and sometimes they could swear they heard whispers from empty rooms. But nothing could have prepared them for the letter that arrived on their eleventh birthday.

The envelope was made of thick, cream-colored parchment, and it was addressed in emerald green ink to ''Alex Morgan, The Small Bedroom, 4 Privet Lane.'' No postage stamp, no return address, just those precise, flowing letters that seemed to shimmer in the morning light.

''Who''s this from?'' Alex''s aunt Martha asked suspiciously, snatching the letter before Alex could open it. She examined it carefully, turning it over in her hands. ''Probably some prank from those neighborhood kids.''

But when she tried to open it, the envelope seemed to seal itself shut, as if held by invisible glue. She pulled and tugged, but it wouldn''t budge. ''Ridiculous,'' she muttered, handing it back to Alex with a frown.

The moment Alex touched the envelope, it opened easily, as if it had been waiting for them all along. Inside was a letter written on the same cream parchment:

Dear Alex,

We are pleased to inform you that you have been accepted at the Arcane Academy of Magical Arts. Please find enclosed a list of all necessary books and equipment. Term begins on September 1st. We await your owl by no later than July 31st.

Yours sincerely,
Professor McGillicuddy
Deputy Headmaster

Alex read the letter three times, their heart pounding with excitement and disbelief. Magic was real? They were a wizard? It explained so much about the strange occurrences throughout their life.

''What does it say?'' Aunt Martha demanded, but when Alex showed her the letter, all she saw was a blank piece of parchment. ''Very funny, Alex. Stop wasting time with silly pranks and go do your chores.''

But Alex knew this was no prank. This was the beginning of the greatest adventure of their life...',
4.8, GETDATE(), GETDATE()),

('Heroes United: The Battle for Earth', 'Stan L. Marvel', '978-0-987-65432-1', 2023, 'Superhero Fiction', 'Hero Comics Press', 450, 0, 1,
'When an alien threat emerges, Earth''s mightiest heroes must unite to defend humanity. Action-packed adventure featuring Iron Knight, Captain Liberty, Thunder God, and the Incredible Green Guardian.',
'The sky over New York City crackled with otherworldly energy as massive ships descended through the clouds. Citizens fled in terror as strange beings in gleaming armor poured from the vessels, their weapons unlike anything Earth had ever seen. This was the moment the world''s greatest heroes had trained for...',
'Chapter 1: The Invasion Begins

The sky over New York City crackled with otherworldly energy as massive ships descended through the clouds. Citizens fled in terror as strange beings in gleaming armor poured from the vessels, their weapons unlike anything Earth had ever seen. This was the moment the world''s greatest heroes had trained for, though none had imagined it would come so suddenly.

Tony Starke was in his workshop when FRIDAY alerted him to the situation. ''Sir, we have multiple unidentified aircraft over Manhattan. Energy signatures are off the charts.''

''Show me,'' Tony commanded, his workshop screens lighting up with satellite feeds. What he saw made his blood run cold. ''FRIDAY, get me Steve Rogers. Now.''

Across the city, Steve Rogers was leading a training session when his communicator buzzed. The moment he saw the footage, he knew their greatest test had arrived. ''Assemble the team,'' he ordered. ''This is what we''ve prepared for.''

Meanwhile, in Asgard, Thor sensed the disturbance across the realms. The Bifrost bridge hummed with power as he prepared to return to Midgard. His friends would need him now more than ever.

Dr. Bruce Banner was lecturing at Columbia University when the first explosions rocked the city. Through the classroom windows, he could see the alien ships unleashing devastating energy beams on the streets below. His students screamed and fled, but Bruce remained calm. He knew what had to be done.

''Everyone stay calm and follow evacuation procedures,'' he announced to his class. But even as he spoke, he could feel the familiar stirring within him. The other guy was ready to come out and play.

Natasha Romanoff was already in position, having been tracking unusual energy readings for the past week. From her vantage point on a Manhattan rooftop, she had a clear view of the invasion force. Her earpiece crackled to life as Nick Fury''s voice cut through the chaos.

''Romanoff, we need you to coordinate ground evacuation while the others engage the primary threat.''

''Copy that,'' she replied, already moving. ''But sir, this is bigger than anything we''ve faced before. We''re going to need everyone.''

As the heroes converged on the battlefield, each brought their unique skills to bear against an enemy unlike any they had faced. The fate of humanity hung in the balance, and only by working together could they hope to prevail...',
4.9, GETDATE(), GETDATE()),

('Wizarding World: Magical Creatures Encyclopedia', 'Luna S. Lovegood', '978-2-345-67890-1', 2022, 'Fantasy Reference', 'Diagon Alley Books', 680, 0, 0,
'A comprehensive guide to magical creatures, their habitats, behaviors, and care. Essential reading for any aspiring magizoologist or curious wizard.',
'Dragons have captivated the magical world for millennia. These magnificent beasts, with their ancient wisdom and fearsome power, represent both the beauty and danger of magic itself. The most common species encountered in Britain is the Common Welsh Green...',
'Chapter 12: Dragons of the British Isles

Dragons have captivated the magical world for millennia. These magnificent beasts, with their ancient wisdom and fearsome power, represent both the beauty and danger of magic itself. The most common species encountered in Britain is the Common Welsh Green, though several other varieties have been documented.

The Common Welsh Green (Draco britannicus) is perhaps the most docile of all dragon species, preferring to nest in the mountainous regions of Wales. Adults typically reach lengths of 18-20 feet, with a wingspan nearly double that measurement. Their distinctive emerald scales shimmer with an inner fire, and their eyes burn with an intelligence that has led many researchers to believe dragons possess a form of magic far older than our own.

Unlike their more aggressive cousins, Welsh Greens rarely attack humans unless directly threatened or protecting their young. They feed primarily on sheep and cattle, though they have been known to supplement their diet with fish from mountain lakes. Their flame burns a brilliant green-white and can reach temperatures of over 2,000 degrees Celsius.

The Norwegian Ridgeback (Draco norwegicus) is considerably more dangerous. These dragons are aggressive by nature and will attack without provocation. They are easily identified by the prominent spines along their backs and their distinctive bronze coloration. Ridgebacks are native to the fjords of Norway but have been spotted as far south as Scotland in recent years.

One of the most fascinating aspects of dragon behavior is their hoarding instinct. All dragons, regardless of species, are compelled to collect objects they find beautiful or valuable. These hoards can contain anything from precious metals and gems to seemingly worthless trinkets that have caught the dragon''s fancy. Some scholars believe this behavior is linked to their magical nature, as many of the items in dragon hoards have been found to possess unique magical properties.

Perhaps the most dangerous dragon species in Europe is the Hungarian Horntail (Draco hungaricus). These black-scaled beasts are known for their aggression and their particular fondness for hunting wizards. Their tail spikes can pierce dragon-hide armor, and their flame burns so hot it appears almost transparent. Horntails are fiercely territorial and will defend their nesting grounds to the death.

Dragon eggs are among the most regulated magical substances in the world. The sale, purchase, or possession of dragon eggs without proper Ministry authorization carries a sentence of life in Azkaban. This is not merely due to their value (a single dragon egg can be worth more than most wizards earn in a lifetime) but because of the extreme danger posed by baby dragons.

Newborn dragons, or dragonets, are born with their full magical abilities intact. They can breathe fire within hours of hatching and will instinctively attack anything they perceive as a threat. Many well-meaning wizards have lost their lives attempting to raise dragons from eggs, underestimating the lethal nature of these creatures even in infancy.

For those brave or foolish enough to encounter a dragon in the wild, remember these essential survival tips: Never make direct eye contact, as dragons consider this a challenge. Move slowly and avoid sudden movements. Most importantly, never attempt to steal from a dragon''s hoard - they can sense the magical signature of their possessions from miles away and will hunt the thief relentlessly.

Dragons remain one of the most magnificent and terrifying creatures in our magical world, deserving of both our respect and our caution...',
4.7, GETDATE(), GETDATE()),

-- I'll continue with the remaining resources in the next part due to length constraints
('Superhero Training Academy: Complete Season 1', 'Marvel Studios Academy', 'DVD-HERO-2024', 2024, 'Superhero Documentary', 'Hero Education Media', 600, 2, 1,
'Follow new recruits as they train to become the next generation of superheroes. Features exclusive interviews with legendary heroes and behind-the-scenes training footage.',
'Welcome to the Superhero Training Academy, where ordinary individuals discover their extraordinary potential. In this exclusive documentary series, we follow twelve recruits through their intensive training program designed to prepare them for the responsibilities of heroism...',
'Episode 1: Welcome to Hero Academy

Welcome to the Superhero Training Academy, where ordinary individuals discover their extraordinary potential. In this exclusive documentary series, we follow twelve recruits through their intensive training program designed to prepare them for the responsibilities of heroism.

Our first recruit is Sarah Chen, a 23-year-old engineer from Seattle who discovered her ability to manipulate electromagnetic fields during a power outage at her office building. ''I thought I was going crazy,'' Sarah explains during her interview. ''Electronics would malfunction around me when I got stressed. Then one day, I accidentally brought down the entire city grid during a panic attack. That''s when S.H.I.E.L.D. found me.''

Training Director Maria Hill explains the Academy''s philosophy: ''Having powers doesn''t make you a hero. It''s what you choose to do with those powers that counts. Our program is designed to teach discipline, responsibility, and most importantly, the moral framework necessary to protect innocent lives.''

The Academy''s facilities are impressive - a converted military base in upstate New York featuring state-of-the-art training equipment, simulation chambers, and specialized environments for testing different types of abilities. The main training hall is a massive warehouse equipped with obstacle courses, combat training areas, and holographic projection systems capable of creating realistic disaster scenarios.

Recruit Marcus Thompson, who possesses enhanced strength and durability, struggles with controlling his power during combat training. ''I keep breaking the equipment,'' he admits after accidentally punching through a reinforced concrete wall. ''Captain Rogers keeps telling me it''s not about being strong - it''s about knowing when not to use that strength.''

The psychological aspects of hero training are just as important as the physical ones. Dr. Helen Cho, the Academy''s chief psychologist, works with recruits to address the mental challenges of heroism. ''Many of our students have experienced trauma related to their abilities,'' she explains. ''Some have accidentally hurt people they care about. Others have been rejected by friends and family. We help them process these experiences and build the emotional resilience necessary for heroic work.''

Perhaps the most challenging aspect of the program is the ethics curriculum, led by Professor Charles Xavier via holographic link from his school in Westchester. ''Every hero will face impossible choices,'' Xavier explains to the recruits during a particularly intense session. ''The question isn''t whether you can save everyone - it''s how you decide who to save when you can''t save them all.''

The episode concludes with the recruits'' first major test - a simulated terrorist attack on a crowded shopping mall. Using holographic technology and advanced robotics, the training staff creates a scenario where recruits must work together to neutralize threats while protecting civilians. The results are mixed, with several recruits making critical errors that would have resulted in civilian casualties in a real situation.

''This is why we train,'' Director Hill reminds them during the debriefing. ''Every mistake you make here is a life you''ll save in the field. The stakes couldn''t be higher, and neither could the honor of what you''re preparing to do.''

Next week: The recruits face their first real-world assignment when a natural disaster strikes nearby, and they must put their training to the test alongside established heroes...',
4.8, GETDATE(), GETDATE()),

('The Dark Arts: A History of Forbidden Magic', 'Severus P. Snape', '978-3-456-78901-2', 2020, 'Dark Fantasy', 'Borgin & Burkes Academic', 890, 0, 1,
'A comprehensive examination of dark magic throughout history. Required reading for advanced Defense Against the Dark Arts students. Contains detailed analysis of curses, hexes, and protective countermeasures.',
'The study of dark magic is not for the faint of heart. Those who delve into these forbidden arts must understand that knowledge itself can be a dangerous weapon. This text serves as both warning and guide for those who must understand darkness to defend against it...',
'Preface: On the Nature of Dark Magic

The study of dark magic is not for the faint of heart. Those who delve into these forbidden arts must understand that knowledge itself can be a dangerous weapon. This text serves as both warning and guide for those who must understand darkness to defend against it.

I have spent decades studying the dark arts, not out of fascination with evil, but from the necessity of understanding one''s enemy. As a professor of Defense Against the Dark Arts, I have seen firsthand the devastation that can be wrought by those who embrace the darkness without understanding its true cost.

Dark magic, at its core, is magic that is performed with the intention to harm, control, or corrupt. It feeds on negative emotions - hatred, fear, anger, despair. The more intense these emotions, the more powerful the magic becomes. This is why dark wizards often appear to grow stronger over time; they become addicted to the rush of power that comes from channeling their darkest impulses.

The Three Unforgivable Curses represent the pinnacle of dark magic, each designed to strip away fundamental human rights. The Imperius Curse removes free will, turning the victim into a puppet. The Cruciatus Curse inflicts unbearable pain while leaving no physical marks, making it a favorite tool of interrogators and sadists. And the Killing Curse... Avada Kedavra ends life itself with no possibility of magical healing or reversal.

What makes these curses truly unforgivable is not just their effects, but what they require of the caster. To successfully perform any of these curses, one must mean them completely. There can be no hesitation, no moral qualms, no trace of humanity left in the moment of casting. The wizard must embrace the darkness fully, and in doing so, they lose a piece of their soul forever.

This is the true danger of dark magic - not just what it does to the victim, but what it does to the caster. Each act of dark magic leaves a mark on the soul, a corruption that grows with every subsequent act. It becomes easier each time, until eventually, the wizard loses the ability to feel empathy, love, or remorse. They become something less than human.

Voldemort serves as the ultimate example of this corruption. Born Tom Marvolo Riddle, he was once a brilliant student, charming and capable. But his obsession with dark magic, his quest for immortality through the creation of Horcruxes, transformed him into something monstrous. By the time of his downfall, he had become barely recognizable as having once been human.

Yet we must study these arts, for only by understanding darkness can we hope to defend against it. The protective spells, the counter-curses, the methods of detection - all require knowledge of what we seek to protect against. A healer must understand poison to create antidotes; a defender must understand dark magic to create protections.

This book contains knowledge that has been carefully compiled over centuries, drawn from the most dangerous texts in wizarding libraries, from the confessions of captured dark wizards, and from my own observations during the wars against Voldemort. I have included detailed instructions for defensive spells, methods for detecting dark magic, and the psychological profiles of those who turn to darkness.

To my students, I offer this warning: knowledge is power, but power without wisdom is destruction. Study these pages well, but never forget the human cost of the magic described within. The moment you begin to see these spells as solutions rather than problems, you have already begun your journey into the darkness.

Remember always that the brightest light casts the darkest shadow, and those who fight monsters must be careful not to become monsters themselves.

- Professor S. Snape
  Hogwarts School of Witchcraft and Wizardry',
4.3, GETDATE(), GETDATE()),

-- === JOURNALS ===
('Journal of Advanced Magical Theory', 'Albus P. Dumbledore', 'ISSN-2045-7890', 2024, 'Academic Journal', 'Wizarding Academic Press', 45, 1, 0,
'Leading academic journal featuring cutting-edge research in magical theory, spell innovation, and theoretical frameworks for understanding magic.',
'This month''s edition explores the revolutionary concept of ''magic as information theory'' - suggesting that spells are essentially data structures that interact with the fundamental fabric of reality...',
'Volume 127, Issue 3 - March 2024

Editorial: Magic as Information Theory
By Professor Hermione Granger-Weasley

This month''s edition explores the revolutionary concept of ''magic as information theory'' - suggesting that spells are essentially data structures that interact with the fundamental fabric of reality. Dr. Luna Lovegood''s groundbreaking research at the Department of Mysteries has opened new avenues for understanding how magical energy can be quantified, stored, and transmitted.

Featured Articles:

1. ''Quantum Entanglement in Paired Wands: A Study of the Elder Wand Phenomenon''
By Garrick Ollivander III

Our research into wand cores has revealed fascinating parallels between magical resonance and quantum mechanical principles. When two wands share material from the same magical creature, they exhibit properties remarkably similar to quantum entanglement. This study follows 847 paired wands over a 50-year period, documenting instances of sympathetic magical discharge and cross-wand spell interference.

The implications are staggering. If wands can indeed maintain quantum-level connections regardless of distance, this could revolutionize long-distance magical communication, defensive ward networks, and even our understanding of apparition itself.

2. ''Temporal Magic: New Theories on Time-Turner Mechanics''
By Dr. Minerva McGonagall

Following the destruction of the Time Room at the Ministry of Magic, researchers have been working to understand how time-manipulation devices actually function. Our analysis suggests that Time-Turners don''t actually send the user backward in time, but rather create localized temporal bubbles where time flows at different rates.

This theory explains several previously inexplicable phenomena, including why time-travelers can interact with their past selves without creating paradoxes, and why extended time-turner use leads to rapid aging. The temporal bubble theory suggests that the user experiences every moment of ''rewound'' time, but in an accelerated state that appears instantaneous to outside observers.

3. ''Magical Creature Intelligence: Beyond Human Understanding''
By Luna S. Lovegood

Our five-year study of dragon behavior has revealed cognitive abilities that challenge our assumptions about magical creature intelligence. Dragons demonstrate not only problem-solving skills equivalent to human eight-year-olds, but also show evidence of abstract thinking, long-term planning, and what can only be described as artistic expression.

The most remarkable discovery came when we observed a group of Welsh Greens arranging their hoard items in complex geometric patterns that correspond to ancient magical symbols. When replicated by human wizards, these patterns produced powerful protective enchantments that we had previously thought impossible.

This suggests that magical creatures may possess intuitive understanding of magical principles that exceeds human knowledge, raising profound questions about the nature of intelligence itself.

Book Reviews:

''Elementary Spell Theory Revised'' by Miranda Goshawk
Reviewed by Professor Filius Flitwick

Goshawk''s latest edition incorporates recent discoveries about wand movement optimization and incantation phonetics. Her analysis of how regional accents affect spell potency is particularly enlightening for international magical education programs.

Upcoming Conferences:

- International Symposium on Magical Ethics (June 15-17, Edinburgh)
- Conference on Experimental Transfiguration (July 22-24, Salem)
- Annual Meeting of the Society for Magical Creature Rights (August 5-7, Brussels)

Call for Papers:

The Journal seeks submissions for our special October issue on ''Magic and Technology Integration.'' We''re particularly interested in research on magical enhancement of Muggle technology and the development of hybrid magical-electronic devices.

Submission deadline: May 31, 2024
For submission guidelines, contact the editorial board at editor@advancedmagicaltheory.org

---

Subscription Information:
Annual subscription (12 issues): 50 Galleons
Student rate: 25 Galleons with valid school ID
Library institutional rate: 200 Galleons

ISSN: 2045-7890
Published monthly by Wizarding Academic Press, Diagon Alley, London',
4.6, GETDATE(), GETDATE()),

('Modern Defense Strategies Quarterly', 'Harry J. Potter', 'ISSN-1987-6543', 2024, 'Military Journal', 'Auror Training Institute', 38, 1, 1,
'Quarterly publication focusing on modern magical defense techniques, threat assessment, and protection strategies for both military and civilian applications.',
'The evolution of dark magic requires constant adaptation of our defensive strategies. This issue examines emerging threats from techno-magical hybrid attacks and the rise of AI-assisted dark magic...',
'Issue 45, Q1 2024

Editor''s Note
By Harry Potter, Senior Auror

The evolution of dark magic requires constant adaptation of our defensive strategies. This issue examines emerging threats from techno-magical hybrid attacks and the rise of AI-assisted dark magic detection systems.

Lead Article: ''Cyber-Magic: When Technology Meets Dark Arts''
By Kingsley Shacklebolt, Minister of Magic

The integration of Muggle technology with magical attacks represents a new frontier in dark arts evolution. Recent incidents involving magically enhanced computer viruses and technologically amplified curses have forced a complete reevaluation of our defensive protocols.

Case Study 1: The London Bridge Incident
Last September, dark wizards successfully infiltrated London''s traffic control systems, using a combination of Confundus Charms and computer programming to cause massive gridlock while they robbed Gringotts Bank. The attack demonstrated how modern dark wizards are adapting to our increasingly connected world.

Traditional magical defenses proved insufficient against this hybrid threat. Anti-technology jinxes couldn''t affect the properly shielded computer systems, while Muggle cybersecurity measures were useless against the magical components of the attack.

Recommended Countermeasures:
- Development of hybrid detection systems combining magical sensors with electronic monitoring
- Cross-training programs for Aurors in both magical and Muggle security techniques
- Establishment of joint task forces with Muggle intelligence agencies

Special Report: ''AI in Magical Law Enforcement''
By Dr. Cho Chang, Department of Magical Research

Artificial Intelligence systems are revolutionizing threat detection in the magical world. The new ATHENA system (Automated Threat Hexing and Enchantment Network Analyzer) can process thousands of magical signatures simultaneously, identifying potential dark magic signatures in real-time.

Field test results from the past six months show a 347% improvement in early threat detection compared to human-only monitoring systems. The AI can recognize patterns that human analysts miss, such as subtle variations in magical energy that indicate curse preparation.

However, AI systems remain vulnerable to magical interference. The Imperius Curse has proven particularly effective against AI decision-making algorithms, requiring the development of new protective protocols.

Tactical Review: ''Urban Magical Combat''
By Neville Longbottom, Defense Against the Dark Arts Professor

Modern magical conflicts increasingly occur in densely populated Muggle areas, requiring new tactical approaches that minimize civilian exposure while maintaining operational effectiveness.

Key considerations for urban magical combat:

1. Concealment Protocols: Traditional Disillusionment Charms are insufficient in areas with extensive CCTV coverage. New multi-spectrum concealment spells are required.

2. Collateral Damage Minimization: Precision curse work is essential. Wide-area spells like Fiendfyre are absolutely prohibited in urban environments.

3. Muggle Memory Management: Large-scale memory modification operations require careful coordination with the Department for Regulation and Control of Magical Creatures'' Obliviation Task Force.

Training Corner: ''Advanced Shield Charm Techniques''
By Professor Luna Lovegood

Recent innovations in shield charm theory have produced remarkable improvements in defensive capability. The new ''adaptive shielding'' technique allows shields to automatically adjust their properties based on incoming threats.

Practice Exercise: Gradient Shield Training
Set up targets at varying distances (5m, 10m, 20m). Cast Protego while visualizing the shield adapting to different threat types:
- Physical projectiles: Dense, rigid barrier
- Curse fire: Heat-dissipating mesh structure  
- Mental intrusion: Reflective surface with built-in confusion elements

Master this technique requires approximately 200 hours of practice, but the results justify the investment. Advanced practitioners report 85% effectiveness against unknown threats.

Equipment Review: Latest Protective Gear
- Dragon-hide armor with integrated cooling charms: 4/5 stars
- Self-sharpening silver daggers for werewolf encounters: 5/5 stars
- Portable ward anchors for rapid perimeter establishment: 3/5 stars (reliability issues)

Upcoming Training Opportunities:
- Advanced Dueling Workshop (Ministry Atrium, March 15)
- Anti-Dark Arts Refresher Course (Hogwarts, April 1-3)
- International Defense Conference (Paris, May 20-22)

Submit articles to: editorial@moderndefense.gov.magic
Next issue theme: ''Magical Terrorism Prevention''
Submission deadline: March 30, 2024',
4.7, GETDATE(), GETDATE()),

-- === MEDIA ===
('The Avengers', 'Marvel Studios', 'DVD-MCU-2012-001', 2012, 'Action/Superhero', 'Walt Disney Studios', 8, 2, 1,
'Earth''s Mightiest Heroes must come together and learn to fight as a team to stop the mischievous Loki and his alien army from enslaving humanity. Special 2-disc collector''s edition with behind-the-scenes content.',
'Disc 1: Feature Film (143 minutes)
Disc 2: Special Features including deleted scenes, gag reel, and director''s commentary...',
'THE AVENGERS - Special Edition DVD Collection

Disc 1: Feature Film
Runtime: 143 minutes
Format: Widescreen (2.35:1)
Audio: English 5.1 Surround, Spanish, French
Subtitles: English, Spanish, French, Audio Description

When an unexpected enemy emerges that threatens global safety and security, Nick Fury, Director of the international peacekeeping agency known as S.H.I.E.L.D., finds himself in need of a team to pull the world back from the brink of disaster. Spanning the globe, a daring recruitment effort begins.

Starring:
- Robert Downey Jr. as Tony Stark/Iron Man
- Chris Evans as Steve Rogers/Captain America  
- Mark Ruffalo as Bruce Banner/Hulk
- Chris Hemsworth as Thor
- Scarlett Johansson as Natasha Romanoff/Black Widow
- Jeremy Renner as Clint Barton/Hawkeye
- Tom Hiddleston as Loki
- Samuel L. Jackson as Nick Fury

Disc 2: Special Features

''Marvel One-Shot: Item 47'' (12 minutes)
Follow the story of what happened to one of the Chitauri weapons left behind after the Battle of New York.

''A Visual Journey'' (22 minutes)
Director Joss Whedon and Marvel Studios President Kevin Feige discuss the challenges of bringing together multiple superhero franchises into one cohesive film.

''The Avengers Initiative: A Marvel Second Screen Experience''
Interactive content that synchronizes with the film for behind-the-scenes information, character profiles, and trivia.

Deleted/Extended Scenes (15 minutes):
- Extended Steve Rogers gym scene
- Alternate opening with Maria Hill
- Additional Coulson/Fury conversation
- Extended Thor/Loki confrontation

Gag Reel (4 minutes)
Hilarious outtakes and bloopers from the cast during filming.

Director''s Audio Commentary
Joss Whedon provides insight into his creative process, the challenges of ensemble filmmaking, and easter eggs hidden throughout the film.

''Soundscapes: The Music of The Avengers'' (8 minutes)
Composer Alan Silvestri discusses creating the epic orchestral score and developing individual themes for each hero.

Concept Art Gallery
Over 100 pieces of concept art showing the evolution of character designs, set pieces, and the massive final battle sequence.

Marketing Archive:
- Theatrical trailers (3)
- TV spots (12)
- International promotional materials
- Poster gallery

Easter Eggs:
- Hidden Shawarma restaurant locations in menu navigation
- Stan Lee cameo compilation
- Marvel Studios logo evolution

Bonus Digital Copy Included
Redeem code for digital download compatible with iTunes, Amazon Prime, Google Play, and VUDU.

Special Packaging:
- Embossed slipcover with holographic Avengers logo
- 32-page photo booklet with production stills
- Exclusive character art cards (6)

Language Options:
Audio: English, Spanish (Latin America), French (Canada)
Subtitles: English SDH, Spanish, French

System Requirements:
Region 1 DVD player or computer with DVD drive
Compatible with all standard DVD players worldwide

Rated PG-13 for intense sequences of sci-fi violence and action throughout, and a mild language.

Copyright 2012 Marvel Studios. All rights reserved.',
4.9, GETDATE(), GETDATE()),

('The Dark Knight', 'Warner Bros. Pictures', 'DVD-DC-2008-002', 2008, 'Action/Crime/Drama', 'Warner Home Video', 12, 2, 0,
'Batman raises the stakes in his war on crime with the help of Lt. Jim Gordon and Harvey Dent. But they soon find themselves prey to a reign of chaos unleashed by a rising criminal mastermind known as the Joker. 2-Disc Special Edition.',
'Disc 1: Feature Film (152 minutes) + Commentary
Disc 2: Extensive special features including ''Gotham Tonight'' mockumentary...',
'THE DARK KNIGHT - 2-Disc Special Edition

Disc 1: The Feature Film
Runtime: 152 minutes
Format: Widescreen (2.40:1)
Audio: English 5.1 Surround Sound, French, Spanish
Subtitles: English, French, Spanish

Batman raises the stakes in his war on crime. With the help of Lt. Jim Gordon and Harvey Dent, Batman sets out to dismantle the remaining criminal organizations that plague the streets. The partnership proves to be effective, but they soon find themselves prey to a reign of chaos unleashed by a rising criminal mastermind known to the terrified citizens of Gotham as the Joker.

Starring:
- Christian Bale as Bruce Wayne/Batman
- Heath Ledger as The Joker (Academy Award Winner - Best Supporting Actor)
- Aaron Eckhart as Harvey Dent/Two-Face
- Michael Caine as Alfred Pennyworth
- Maggie Gyllenhaal as Rachel Dawes
- Gary Oldman as Lt. James Gordon
- Morgan Freeman as Lucius Fox

Directed by Christopher Nolan

Disc 2: Special Features

''Gotham Tonight'' (42 minutes)
A six-part mockumentary news program set in Gotham City, featuring interviews with key characters and exploring the city''s reaction to Batman and the Joker''s emergence.

''Focus Points'' - Interactive Feature
Access behind-the-scenes content while watching the film, including:
- The psychology of the Joker
- Practical effects vs. CGI
- Filming locations in Chicago
- Heath Ledger''s preparation for the role

Character Profiles (25 minutes):
- ''The Joker: Concept to Character'' - Heath Ledger''s transformative performance
- ''Harvey Dent: The White Knight''s Fall'' - Aaron Eckhart discusses Two-Face
- ''Batman: Beyond the Mask'' - Christian Bale on evolving the character

''The Making of The Dark Knight'' (30 minutes)
Comprehensive behind-the-scenes documentary covering:
- Pre-production and screenplay development
- Casting Heath Ledger as the Joker
- Filming the truck flip sequence
- Creating Two-Face''s makeup effects
- The IMAX experience

Deleted Scenes (8 minutes):
- Extended bank robbery opening
- Additional Joker interrogation footage
- Alternate Harvey Dent press conference
- Extended hospital explosion sequence

''The Dark Knight IMAX Experience'' (20 minutes)
Exploring Christopher Nolan''s groundbreaking use of IMAX cameras for key action sequences, including the opening bank heist and the truck chase.

''Batman Tech'' (12 minutes)
A look at the real-world inspiration behind Batman''s gadgets and vehicles, featuring interviews with military and technology experts.

''Batman Unmasked: The Psychology of the Dark Knight'' (15 minutes)
Psychologists and comic book experts analyze the psychological aspects of Batman''s war on crime and the Joker''s chaos philosophy.

Production Gallery:
- Concept art and storyboards
- Behind-the-scenes photography
- Costume and makeup tests
- Location scouting photos

Marketing Archive:
- Theatrical trailers (4)
- TV spots and international promos (15)
- Viral marketing content from ''Why So Serious?'' campaign
- Poster gallery

Audio Commentary Options:
- Director Christopher Nolan and cinematographer Wally Pfister
- Production team commentary with producers and key crew members

Easter Eggs:
- Hidden Joker playing cards in menu navigation
- Alternate menu designs based on chaos vs. order theme
- Secret Harvey Dent campaign videos

Bonus Features:
- Digital copy download code
- Exclusive comic book: ''Gotham Knight'' preview
- Limited edition collector''s booklet (16 pages)

Technical Specifications:
Aspect Ratio: 2.40:1 (with IMAX sequences in 1.43:1)
Audio: English 5.1 Dolby Digital, French 5.1, Spanish 5.1
Subtitles: English SDH, French, Spanish
Region: Region 1 (US/Canada)

Rated PG-13 for intense sequences of violence and some menace.

Total Runtime (with special features): Over 6 hours

Copyright 2008 Warner Bros. Entertainment Inc. All rights reserved.',
4.8, GETDATE(), GETDATE()),

('Inception', 'Warner Bros. Pictures', 'BLU-2010-INCEPTION', 2010, 'Sci-Fi/Thriller', 'Warner Home Video', 15, 2, 1,
'Dom Cobb is a skilled thief who specializes in extraction - stealing secrets from deep within the subconscious during the dream state. Now he must perform the impossible: inception. Blu-ray Collector''s Edition with extensive bonus content.',
'Blu-ray Feature Film (148 minutes) in stunning 1080p HD
Extensive special features exploring the science of dreams and practical effects...',
'INCEPTION - Blu-ray Collector''s Edition

Main Feature:
Runtime: 148 minutes
Format: 1080p High Definition (2.40:1)
Audio: English DTS-HD Master Audio 5.1, French DTS 5.1, Spanish DTS 5.1
Subtitles: English SDH, French, Spanish, Portuguese

Dom Cobb is a skilled thief, the absolute best in the dangerous art of extraction, stealing valuable secrets from deep within the subconscious during the dream state, when the mind is at its most vulnerable. Cobb''s rare ability has made him a coveted player in this treacherous new world of corporate espionage, but it has also made him an international fugitive and cost him everything he has ever loved.

Now Cobb is being offered a chance at redemption. One last job could give him his life back but only if he can accomplish the impossible - inception. Instead of the perfect heist, Cobb and his team of specialists have to pull off the reverse: their task is not to steal an idea but to plant one.

Starring:
- Leonardo DiCaprio as Dom Cobb
- Ken Watanabe as Saito
- Joseph Gordon-Levitt as Arthur
- Marion Cotillard as Mal
- Ellen Page as Ariadne
- Tom Hardy as Eames
- Cillian Murphy as Robert Fischer
- Michael Caine as Professor Miles

Written and Directed by Christopher Nolan

Special Features:

''Extraction Mode'' - Enhanced Viewing Experience
Watch the film with picture-in-picture behind-the-scenes content, including:
- Cast and crew interviews
- Production photos and concept art
- Technical breakdowns of complex sequences
- Dream level navigation guide

''Dreams: Cinema of the Subconscious'' (44 minutes)
A comprehensive documentary exploring:
- The science of lucid dreaming
- Christopher Nolan''s inspiration and creative process
- The film''s complex narrative structure
- Practical effects vs. digital enhancement

''Inception: The Cobol Job'' (14 minutes)
Animated prequel comic explaining the backstory of Dom Cobb''s relationship with Cobol Engineering and the events leading up to the film.

''5.1 Ideas: The Inception of Christopher Nolan'' (35 minutes)
In-depth interview with director Christopher Nolan discussing:
- The 10-year development process
- Influences from classic heist films
- Working with IMAX cameras
- The film''s ending and interpretation

Concept and Production Galleries:
- Pre-visualization sequences
- Storyboard comparisons
- Costume design evolution
- Set construction time-lapse
- Location scouting worldwide

''Constructing Paradox: The Production Design'' (22 minutes)
Production designer Guy Hendrix Dyas discusses creating the film''s multiple dream worlds:
- The hotel corridor fight sequence
- Building Limbo''s architecture
- The spinning hallway construction
- Creating impossible geometries

''The Big Idea: Altering Architecture'' (16 minutes)
Special effects supervisor Paul Franklin explains the practical and digital effects:
- The folding Paris sequence
- Zero gravity fight choreography
- The train in downtown Los Angeles
- Limbo''s crumbling buildings

''Inception: 4 Levels of Dreams'' Interactive Map
Navigate through the film''s complex dream structure with:
- Character tracking across levels
- Time dilation explanations
- Kick synchronization breakdown
- Totem significance guide

Deleted and Extended Scenes (12 minutes):
- Extended Mombasa chase sequence
- Additional Mal backstory
- Alternate limbo conversations
- Extended fortress assault

Marketing Archive:
- Theatrical trailers (3)
- TV spots (8)
- International promotional content
- Mind Crime viral videos
- Poster gallery

Audio Commentary:
- Christopher Nolan (Director/Writer)
- Wally Pfister (Cinematographer) 
- Lee Smith (Editor)
- Guy Hendrix Dyas (Production Designer)

Technical Specifications:
Video: 1080p/AVC MPEG-4 (2.40:1)
Audio: English DTS-HD Master Audio 5.1
Subtitles: Multiple languages available
Region: Region A (Americas, East Asia)
Discs: 2 (1 Blu-ray, 1 DVD, 1 Digital Copy)

Packaging:
- Collectible SteelBook case
- 40-page production booklet
- Exclusive concept art postcards (8)
- Digital copy ultraviolet code

Bonus Digital Features:
- BD-Live exclusive content
- Warner Bros. movie trailers
- Enhanced UltraViolet experience

Rated PG-13 for sequences of violence and action throughout.

Total Special Features Runtime: Over 4 hours

Copyright 2010 Warner Bros. Entertainment Inc. All rights reserved.',
4.9, GETDATE(), GETDATE()),

-- === ADDITIONAL BOOKS ===
('The Hobbit''s Great Adventure', 'R.R. Tolkien', '978-5-555-11111-1', 1937, 'Fantasy', 'Fantasy Classics', 300, 0, 1,
'A classic tale of adventure featuring a reluctant hero''s journey through magical lands.',
'In a hole in the ground there lived a hobbit. Not a nasty, dirty, wet hole filled with worms and oozing smells, but a comfortable hobbit-hole with round doors and windows...',
'Chapter 1: An Unexpected Journey

Bilbo Baggins was a hobbit who enjoyed the quiet life. His comfortable home, good food, and predictable routine suited him perfectly. But on one particular morning, his world was about to change forever.

A knock at the door interrupted his second breakfast. Standing on his doorstep was an old man with a pointed hat and a long grey beard.

''Good morning!'' said the stranger cheerfully.

''What do you mean?'' asked Bilbo, quite flustered. ''Do you wish me a good morning, or mean that it is a good morning whether I want it or not, or that you feel good this morning, or that it is a morning to be good on?''

The old man chuckled. ''All of them at once, I suppose. I am looking for someone to share in an adventure that I am arranging, and it''s very difficult to find anyone suitable.''

''Adventures?'' Bilbo spluttered. ''Nasty disturbing uncomfortable things! Make you late for dinner! I don''t see what anybody sees in them.''

But despite his protests, Bilbo found himself drawn into a tale of distant mountains, sleeping dragons, and lost treasure. Before he knew it, he was signing a contract to serve as a ''burglar'' for a company of dwarves on a quest to reclaim their homeland.

As he packed his bags with trembling hands, Bilbo wondered what he had gotten himself into. Little did he know that this journey would transform him from a timid homebody into one of the bravest adventurers in all of Middle-earth.',
4.8, GETDATE(), GETDATE()),

('Pride and Modern Prejudice', 'Jane Classic', '978-6-666-22222-2', 1813, 'Romance', 'Classic Literature Ltd', 280, 0, 1,
'A timeless story of love, misunderstanding, and social expectations in Regency England.',
'It is a truth universally acknowledged that a single man in possession of a good fortune must be in want of a wife. However little known the feelings or views of such a man may be...',
'Chapter 1: First Impressions

It is a truth universally acknowledged that a single man in possession of a good fortune must be in want of a wife. However little known the feelings or views of such a man may be on his first entering a neighbourhood, this truth is so well fixed in the minds of the surrounding families, that he is considered as the rightful property of some one or other of their daughters.

Elizabeth Bennet had heard this maxim many times from her mother, but she found it rather tiresome. At twenty years old, she was more interested in books and long walks than in the marriage market that seemed to consume the thoughts of everyone around her.

''My dear Mr. Bennet,'' said his lady to him one day, ''have you heard that Netherfield Park is let at last?''

Mr. Bennet replied that he had not.

''But it is,'' returned she; ''for Mrs. Long has just been here, and she told me all about it. A young man of large fortune from the north of England has taken it. His name is Bingley, and he is single!''

Elizabeth rolled her eyes at her mother''s obvious excitement. Another wealthy gentleman to be paraded before her and her sisters like merchandise at market. She much preferred the company of her books to the artificial conversations that such social situations demanded.

Little did she know that this Mr. Bingley would bring with him a friend whose pride would clash spectacularly with her own prejudices, setting in motion a series of misunderstandings that would challenge everything she thought she knew about love and human nature.',
4.7, GETDATE(), GETDATE()),

('1984: A Dystopian Future', 'George Orwell', '978-7-777-33333-3', 1949, 'Dystopian Fiction', 'Political Press', 320, 0, 1,
'A chilling vision of a totalitarian future where freedom is forbidden and truth is manipulated.',
'It was a bright cold day in April, and the clocks were striking thirteen. Winston Smith, his chin nuzzled into his breast in an effort to escape the vile wind, slipped quickly through the glass doors...',
'Chapter 1: The Ministry of Truth

It was a bright cold day in April, and the clocks were striking thirteen. Winston Smith, his chin nuzzled into his breast in an effort to escape the vile wind, slipped quickly through the glass doors of Victory Mansions, though not quickly enough to prevent a swirl of gritty dust from entering along with him.

The hallway smelt of boiled cabbage and old rag mats. At one end of it a coloured poster, too large for indoor display, had been tacked to the wall. It depicted simply an enormous face, more than a metre wide: the face of a man of about forty-five, with a heavy black moustache and ruggedly handsome features.

Winston made for the stairs. It was no use trying the lift. Even at the best of times it was seldom working, and at present the electric current was cut off during daylight hours. It was part of the economy drive in preparation for Hate Week.

The flat was seven flights up, and Winston, who was thirty-nine and had a varicose ulcer above his right ankle, went slowly, resting several times on the way. On each landing, opposite the lift shaft, the poster with the enormous face gazed from the wall. It was one of those pictures which are so contrived that the eyes follow you about when you move.

''BIG BROTHER IS WATCHING YOU,'' the caption beneath it ran.

Inside the flat a fruity voice was reading out a list of figures which had something to do with the production of pig-iron. The voice came from an oblong metal plaque like a dulled mirror which formed part of the surface of the right-hand wall.

Winston turned a switch and the voice sank somewhat, though the words were still distinguishable. The instrument (the telescreen, it was called) could be dimmed, but there was no way of shutting it off completely.',
4.6, GETDATE(), GETDATE()),

('To Kill a Mockingbird''s Legacy', 'Harper Lee', '978-8-888-44444-4', 1960, 'Social Fiction', 'Justice Publishing', 290, 0, 1,
'A powerful story about justice, morality, and growing up in the American South during the 1930s.',
'When I was almost six and Jem was nearly ten, our summertime boundaries were Mrs. Henry Lafayette Dubose''s house two doors to the north of us, and the Radley Place three doors to the south...',
'Chapter 1: Maycomb County

When I was almost six and Jem was nearly ten, our summertime boundaries were Mrs. Henry Lafayette Dubose''s house two doors to the north of us, and the Radley Place three doors to the south. We were never tempted to break them, for the Radley Place was inhabited by an unknown entity the mere description of whom was enough to make us behave for days on end.

Maycomb was an old town, but it was a tired old town when I first knew it. In rainy weather the streets turned to red slop; grass grew on the sidewalks, the courthouse sagged in the square. Somehow, it was hotter then: a black dog suffered on a summer''s day; bony mules hitched to Hoover carts flicked flies in the sweltering shade of the live oaks on the square.

Men''s stiff collars wilted by nine in the morning. Ladies bathed before noon, after their three-o''clock naps, and by nightfall were like soft teacakes with frostings of sweat and sweet talcum.

People moved slowly then. They ambled across the square, shuffled in and out of the stores around it, took their time about everything. A day was twenty-four hours long but seemed longer. There was no hurry, for there was nowhere to go, nothing to buy and no money to buy it with, nothing to see outside the boundaries of Maycomb County.

But it was a time of vague optimism for some of the people: Maycomb County had recently been told that it had nothing to fear but fear itself.',
4.8, GETDATE(), GETDATE()),

('The Great Gatsby''s Era', 'F. Scott Fitzgerald', '978-9-999-55555-5', 1925, 'American Literature', 'Jazz Age Books', 250, 0, 1,
'A classic tale of the American Dream set during the Jazz Age of the 1920s.',
'In my younger and more vulnerable years my father gave me some advice that I''ve carried with me ever since. ''Whenever you feel like criticizing anyone,'' he told me, ''just remember that all the people in this world haven''t had the advantages that you''ve had.''',
'Chapter 1: West Egg

In my younger and more vulnerable years my father gave me some advice that I''ve carried with me ever since. ''Whenever you feel like criticizing anyone,'' he told me, ''just remember that all the people in this world haven''t had the advantages that you''ve had.''

He didn''t say any more, but we''ve always been unusually communicative in a reserved way, and I understood that he meant a great deal more than that. In consequence, I''m inclined to reserve all judgments, a habit that has opened up many curious natures to me and also made me the victim of not a few veteran bores.

And, after boasting this way of my tolerance, I come to the admission that it has a limit. Conduct may be founded on the hard rock or the wet marshes, but after a certain point I don''t care what it''s founded on. When I came back from the East last autumn I felt that I wanted the world to be in uniform and at a sort of moral attention forever; I wanted no more riotous excursions with privileged glimpses into the human heart.

Only Gatsby, the man who gives his name to this book, was exempt from my reaction—Gatsby, who represented everything for which I have an unaffected scorn. If personality is an unbroken series of successful gestures, then there was something gorgeous about him, some heightened sensitivity to the promises of life.',
4.7, GETDATE(), GETDATE()),

-- === ADDITIONAL JOURNALS ===
('Journal of Computer Science Research', 'Dr. Alan Turing', 'ISSN-3333-9999', 2024, 'Computer Science', 'Tech Academic Press', 52, 1, 1,
'Leading publication featuring cutting-edge research in artificial intelligence, machine learning, and computational theory.',
'This edition explores breakthrough developments in quantum computing and its applications to cryptography and optimization problems...',
'Volume 45, Issue 2 - April 2024

Editor''s Note: The Future of Quantum Computing
By Dr. Alan Turing

This edition explores breakthrough developments in quantum computing and its applications to cryptography and optimization problems. Recent advances in quantum error correction have brought us closer to practical quantum computers that could revolutionize computing as we know it.

Featured Research Articles:

1. ''Quantum Machine Learning: Beyond Classical Limitations''
By Dr. Marie Curie, MIT

Our research demonstrates that quantum machine learning algorithms can achieve exponential speedups over classical methods for certain types of pattern recognition problems. We present a novel quantum neural network architecture that successfully classifies complex datasets with unprecedented accuracy.

Key findings:
- Quantum neural networks show 300% improvement in processing speed
- Error rates reduced by 85% compared to traditional algorithms
- Scalable architecture supports datasets with millions of parameters

2. ''Blockchain Security in the Quantum Era''
By Dr. Satoshi Nakamoto, Stanford University

As quantum computers threaten current cryptographic systems, we must develop quantum-resistant blockchain technologies. This paper presents a new consensus mechanism that remains secure even against quantum attacks.

Our proposed solution:
- Post-quantum cryptographic signatures
- Lattice-based hash functions
- Quantum-resistant proof-of-work algorithms

3. ''AI Ethics: Designing Responsible Machine Learning Systems''
By Dr. Ada Lovelace, Oxford University

The rapid advancement of AI technology raises critical ethical questions about bias, privacy, and accountability. This study examines current approaches to ethical AI development and proposes new frameworks for responsible innovation.

Ethical considerations:
- Algorithmic fairness across diverse populations
- Privacy-preserving machine learning techniques
- Transparent decision-making processes
- Human oversight and control mechanisms

Technical Notes:

''Optimizing Database Performance with AI''
By Dr. Edgar Codd, IBM Research

Modern databases can leverage machine learning to automatically optimize query performance and resource allocation. Our experimental system shows 40% improvement in query response times.

Book Reviews:

''Artificial Intelligence: A Modern Approach (5th Edition)''
Reviewed by Dr. John McCarthy

The latest edition incorporates recent developments in deep learning and reinforcement learning. Essential reading for anyone serious about AI research.

Upcoming Conferences:

- International Conference on Machine Learning (ICML 2024) - July 15-18, Vienna
- Conference on Neural Information Processing Systems (NeurIPS 2024) - December 10-16, Vancouver
- Quantum Computing Symposium - September 5-7, Tokyo

Call for Papers:

Special issue on ''Explainable AI in Healthcare'' - Submission deadline: June 30, 2024
We seek original research on developing interpretable machine learning models for medical diagnosis and treatment recommendation.

Subscription Information:
Annual subscription: $120 (Digital) / $180 (Print + Digital)
Student rate: $60 with valid academic ID
Institutional rate: $500

ISSN: 3333-9999
Published bi-monthly by Tech Academic Press',
4.5, GETDATE(), GETDATE()),

('International Medical Research Quarterly', 'Dr. Florence Nightingale', 'ISSN-4444-7777', 2024, 'Medical Science', 'Global Health Publications', 48, 1, 1,
'Prestigious medical journal featuring groundbreaking research in clinical medicine, public health, and biomedical sciences.',
'This quarter''s focus examines revolutionary gene therapy treatments and their potential to cure previously incurable diseases...',
'Volume 78, Issue 1 - Q1 2024

Editorial: Gene Therapy - The Next Medical Revolution
By Dr. Florence Nightingale, Editor-in-Chief

This quarter''s focus examines revolutionary gene therapy treatments and their potential to cure previously incurable diseases. Recent clinical trials have shown remarkable success in treating genetic disorders, cancer, and even some viral infections.

Lead Articles:

1. ''CRISPR-Cas9 Treatment for Sickle Cell Disease: 12-Month Follow-up''
By Dr. Jennifer Doudna, University of California

Our landmark clinical trial followed 50 patients with severe sickle cell disease who received CRISPR-based gene editing treatment. Results show complete remission in 94% of patients with no significant adverse effects.

Patient outcomes:
- Zero pain crises reported in 47 of 50 patients
- Hemoglobin levels normalized within 3 months
- No blood transfusions required post-treatment
- Quality of life scores improved by 340%

2. ''Immunotherapy Breakthrough: Training T-Cells to Fight Pancreatic Cancer''
By Dr. James Allison, MD Anderson Cancer Center

Pancreatic cancer has long been considered one of the most challenging malignancies to treat. Our new CAR-T cell therapy shows unprecedented success rates in advanced-stage patients.

Clinical trial results:
- 67% of patients showed tumor shrinkage
- Median survival increased from 6 to 18 months
- 23% achieved complete remission
- Treatment well-tolerated with manageable side effects

3. ''Global Impact of Malaria Vaccine Rollout in Sub-Saharan Africa''
By Dr. Anthony Fauci, WHO Collaborative Team

The widespread deployment of the new malaria vaccine has dramatically reduced infection rates across 15 African nations. This comprehensive analysis covers implementation challenges and remarkable health outcomes.

Public health impact:
- 78% reduction in malaria cases among vaccinated children
- Hospital admissions decreased by 65%
- Childhood mortality from malaria reduced by 82%
- Cost-effective implementation model developed

Case Studies:

''Telemedicine in Rural Healthcare: Lessons from the Pandemic''
By Dr. Atul Gawande, Harvard Medical School

COVID-19 accelerated telemedicine adoption worldwide. This analysis examines which practices should be permanently integrated into healthcare delivery systems.

Research Notes:

''AI-Assisted Radiology: Improving Diagnostic Accuracy''
By Dr. Geoffrey Hinton, Google Health

Machine learning algorithms now outperform human radiologists in detecting certain types of cancer from medical imaging. We explore the implications for clinical practice.

Pharmacology Update:

''New Alzheimer''s Drug Shows Promise in Phase III Trials''
By Dr. Dale Schenk, Biogen Research

Aducanumab demonstrates significant cognitive improvement in early-stage Alzheimer''s patients. FDA approval expected pending safety review.

Global Health Perspective:

''Addressing Healthcare Inequality: A Systems Approach''
By Dr. Paul Farmer, Partners in Health

Healthcare disparities persist worldwide. This comprehensive framework outlines strategies for delivering quality care to underserved populations.

Upcoming Medical Conferences:

- World Health Assembly - May 22-30, Geneva
- American Medical Association Annual Meeting - June 8-12, Chicago
- International Conference on Global Health - August 15-17, London

Grant Opportunities:

National Institutes of Health seeks applications for precision medicine research - Deadline: May 15, 2024
Gates Foundation announces $50M initiative for infectious disease prevention - Applications open April 1

Subscription Information:
Annual subscription: $200 (Digital) / $300 (Print + Digital)
Resident/Student rate: $75 with verification
Library institutional rate: $800

ISSN: 4444-7777
Published quarterly by Global Health Publications',
4.8, GETDATE(), GETDATE()),

-- === ADDITIONAL MEDIA ===
('The Matrix', 'Warner Bros. Pictures', 'DVD-1999-MATRIX-001', 1999, 'Sci-Fi/Action', 'Warner Home Video', 10, 2, 1,
'Neo discovers that reality as he knows it is actually a computer simulation. He must choose between the blissful ignorance of illusion and the painful truth of reality. Special Edition with groundbreaking behind-the-scenes content.',
'Feature Film (136 minutes) + Extensive special features exploring groundbreaking visual effects and philosophical themes...',
'THE MATRIX - Special Edition DVD

Feature Film:
Runtime: 136 minutes
Format: Widescreen (2.35:1)
Audio: English 5.1 Dolby Digital, French, Spanish
Subtitles: English, French, Spanish

When a beautiful stranger leads computer hacker Neo to a forbidding underworld, he discovers the shocking truth - the life he knows is the elaborate deception of an evil cyber-intelligence.

Starring:
- Keanu Reeves as Neo/Thomas Anderson
- Laurence Fishburne as Morpheus
- Carrie-Anne Moss as Trinity
- Hugo Weaving as Agent Smith
- Joe Pantoliano as Cypher
- Marcus Chong as Tank

Directed by The Wachowski Brothers

Special Features:

''The Philosophy of The Matrix'' (28 minutes)
Scholars and philosophers discuss the film''s exploration of:
- Plato''s Cave allegory
- Descartes'' reality questioning
- Buddhist concepts of illusion (Maya)
- Gnostic religious themes
- Simulation theory and modern philosophy

''Making The Matrix'' (26 minutes)
Behind-the-scenes documentary covering:
- Conceptual development and storyboarding
- Groundbreaking ''bullet time'' effect creation
- Wire work and martial arts training
- Set construction for Nebuchadnezzar ship
- Creating the green digital rain effect

''The Burly Man Chronicles'' (85 minutes)
Exclusive documentary following the production from start to finish:
- Pre-production planning and design
- Casting process and actor preparation
- Daily filming challenges and solutions
- Post-production visual effects work
- Marketing and release strategy

''Follow the White Rabbit'' - Enhanced Viewing Mode
Interactive feature allowing viewers to access behind-the-scenes content during specific scenes:
- Storyboard comparisons
- Alternate takes and angles
- Technical explanations
- Cast and crew commentary

Deleted Scenes (15 minutes):
- Extended opening Trinity chase
- Additional Nebuchadnezzar crew interactions
- Alternate Agent interrogation sequences
- Extended subway fight with Agent Smith

''The Matrix: What Is Bullet Time?'' (8 minutes)
Technical breakdown of the revolutionary visual effect:
- Camera rig construction
- Filming process and timing
- Digital compositing techniques
- Impact on future filmmaking

Concept Art Gallery:
- Character design evolution
- Set and costume concepts
- Storyboard artwork
- The Matrix code visual development

Music Videos:
- ''Wake Up'' by Rage Against the Machine
- ''Rock Is Dead'' by Marilyn Manson
- Behind-the-scenes music video footage

Web Links:
- Official Matrix website access
- Cast and crew filmographies
- Technical specifications
- Awards and recognition

Easter Eggs:
- Hidden philosophical quotes in menu navigation
- Matrix code screensavers
- Red pill / blue pill menu choices
- Agent Smith virus simulation

Audio Commentary:
- The Wachowski Brothers (Directors/Writers)
- Cast commentary with Keanu Reeves and Carrie-Anne Moss
- Technical commentary with visual effects supervisors

Marketing Archive:
- Original theatrical trailers (3)
- TV spots and promotional content (12)
- International advertising materials
- Poster collection

Technical Specifications:
Aspect Ratio: 2.35:1 Anamorphic Widescreen
Audio: English 5.1 Dolby Digital, French 2.0, Spanish 2.0
Subtitles: English, French, Spanish, Portuguese
Region: Region 1 (US/Canada)
Layers: Dual Layer

Special Packaging:
- Collectible keepcase with holographic slipcover
- 16-page booklet with production notes
- Character profile cards (4)

System Requirements:
DVD player or computer DVD drive
Dolby Digital compatible sound system for full audio experience

Rated R for sci-fi violence and brief language.

Total Runtime (with special features): Over 5 hours

Copyright 1999 Warner Bros. Pictures. All rights reserved.',
4.7, GETDATE(), GETDATE()),

('Interstellar', 'Paramount Pictures', 'BLU-2014-INTERSTELLAR', 2014, 'Sci-Fi/Drama', 'Paramount Home Entertainment', 12, 2, 1,
'In Earth''s future, a global crop blight and second Dust Bowl are slowly rendering the planet uninhabitable. Cooper, an ex-NASA pilot turned farmer, is tasked to pilot a spacecraft to find mankind a new home. Limited Edition Blu-ray with extensive scientific bonus content.',
'Feature Film (169 minutes) in stunning 4K quality + Scientific documentaries exploring real space science...',
'INTERSTELLAR - Limited Edition Blu-ray

Main Feature:
Runtime: 169 minutes
Format: 1080p High Definition (2.40:1)
Audio: English DTS-HD Master Audio 5.1, French DTS 5.1, Spanish DTS 5.1
Subtitles: English SDH, French, Spanish, Portuguese, Mandarin

In Earth''s future, a global crop blight and second Dust Bowl are slowly rendering the planet uninhabitable. Professor Brand, a brilliant NASA physicist, is working on plans to save mankind by transporting Earth''s population to a new home via a wormhole. But first, Brand must send former NASA pilot Cooper and a team of researchers through the wormhole and across the galaxy to find out which of three planets could be mankind''s new home.

Starring:
- Matthew McConaughey as Cooper
- Anne Hathaway as Dr. Amelia Brand
- Jessica Chastain as Murph (adult)
- Michael Caine as Professor Brand
- Casey Affleck as Tom Cooper
- Wes Bentley as Doyle
- Matt Damon as Dr. Mann
- Mackenzie Foy as Murph (child)

Directed by Christopher Nolan

Special Features:

''The Science of Interstellar'' (51 minutes)
Physicist Kip Thorne, who served as executive producer and scientific consultant, explains:
- Black hole physics and visualization
- Time dilation and relativity theory
- Wormhole theoretical construction
- The accuracy of Gargantua''s depiction
- Fifth-dimensional space concepts

''Plotting an Interstellar Journey'' (24 minutes)
Christopher Nolan discusses:
- The film''s scientific foundation
- Collaboration with Kip Thorne
- Balancing science with storytelling
- Practical effects vs. digital enhancement
- IMAX filming techniques

''Life on Cooper''s Farm'' (12 minutes)
Exploring the film''s Earth-based sequences:
- Creating the dust bowl environment
- Corn field cultivation for filming
- Production design of Cooper''s farmhouse
- Capturing the agricultural crisis

''The Dust'' (8 minutes)
Special effects breakdown of the omnipresent dust storms:
- Practical dust effect creation
- Digital enhancement techniques
- Safety protocols during filming
- Environmental storytelling through dust

''TARS and CASE: Designing the Robots'' (17 minutes)
The creation of the film''s unique robot characters:
- Physical puppet construction
- Movement and puppeteer coordination
- Voice recording and processing
- Character personality development

''Cosmic Sounds: The Music of Interstellar'' (15 minutes)
Composer Hans Zimmer discusses:
- Organ-based musical themes
- Emotional resonance in space
- Recording in unique acoustic environments
- The ''No Time for Caution'' docking sequence score

''The Space Suits'' (6 minutes)
Costume design and practical considerations:
- NASA consultation for authenticity
- Actor mobility and comfort
- Helmet communication systems
- Zero-G movement simulation

''Shooting in Iceland: Miller''s Planet'' (14 minutes)
Location filming on the ice planet:
- Scouting remote Icelandic locations
- Extreme weather filming challenges
- Creating otherworldly landscapes
- Practical water effects

''The Endurance: Spacecraft Design'' (11 minutes)
Production design of the main spacecraft:
- Modular construction philosophy
- Rotating sections and artificial gravity
- Set construction and camera movement
- Interior lighting and atmosphere

''Visualizing the Wormhole and Black Hole'' (19 minutes)
Groundbreaking visual effects creation:
- Scientific accuracy in rendering
- New software development for Gargantua
- Light bending and gravitational lensing
- The accretion disk''s realistic appearance

Deleted and Extended Scenes (23 minutes):
- Extended Cooper family breakfast
- Additional Earth''s dying environment footage
- Longer Dr. Mann confrontation
- Alternate tesseract sequence takes

''Theoretical Astrophysics: Kip Thorne''s Insights'' (31 minutes)
Advanced scientific discussion covering:
- Warped spacetime visualization
- Closed timelike curves
- Higher dimensional physics
- The intersection of science and cinema

Audio Commentary:
- Christopher Nolan (Director/Writer)
- Jonathan Nolan (Writer)
- Kip Thorne (Executive Producer/Scientific Consultant)
- Hoyte van Hoytema (Cinematographer)

IMAX Enhanced Version:
- Select sequences presented in expanded aspect ratio
- Enhanced detail and scope for home viewing
- Director''s preferred presentation format

Marketing Archive:
- Theatrical trailers (4)
- TV spots and promotional content (15)
- International marketing materials
- Scientific promotional videos
- Poster gallery

Technical Specifications:
Video: 1080p/AVC MPEG-4 (2.40:1)
Audio: English DTS-HD Master Audio 5.1
Subtitles: Multiple languages available
Region: Region A (Americas, East Asia)
Discs: 3 (2 Blu-ray, 1 DVD)

Packaging:
- Premium digipak case with magnetic closure
- 64-page companion booklet with scientific essays
- Exclusive concept art prints (6)
- Digital HD UltraViolet copy included

Bonus Digital Content:
- Interactive periodic table
- NASA partnership content
- Scientific accuracy comparisons
- Real space mission footage

Rated PG-13 for some intense perilous action and brief strong language.

Total Special Features Runtime: Over 4 hours

Copyright 2014 Paramount Pictures Corporation. All rights reserved.',
4.8, GETDATE(), GETDATE());
GO

-- Insert sample borrow records matching EF seed data exactly
SET IDENTITY_INSERT BorrowRecords ON;

INSERT INTO BorrowRecords (Id, ResourceId, BorrowerName, BorrowDate, DueDate, ReturnDate, IsReturned) VALUES
-- alex's borrowing history - mix of returned and current
(1, 1, 'alex', '2025-07-15', '2025-07-29', '2025-07-28', 1), -- The Young Wizard's Journey - returned
(2, 8, 'alex', '2025-07-20', '2025-08-03', '2025-08-01', 1), -- The Avengers - returned
(3, 3, 'alex', '2025-07-25', '2025-08-08', NULL, 0), -- Magical Creatures Encyclopedia - currently borrowed

-- ashu's borrowing history - includes one overdue
(4, 2, 'ashu', '2025-07-10', '2025-07-24', '2025-07-23', 1), -- Heroes United - returned
(5, 6, 'ashu', '2025-07-18', '2025-08-01', NULL, 0), -- Journal of Advanced Magical Theory - currently borrowed
(6, 9, 'ashu', '2025-07-12', '2025-07-26', NULL, 0), -- The Dark Knight - overdue
(7, 13, 'ashu', '2025-07-22', '2025-08-05', '2025-08-04', 1); -- 1984: A Dystopian Future - returned

SET IDENTITY_INSERT BorrowRecords OFF;
GO

-- Insert sample reading sessions
INSERT INTO ReadingSessions (ResourceId, ReaderName, StartTime, CurrentPage, TotalPages, IsCompleted, BookmarkPosition) VALUES
(1, 'Josh', DATEADD(day, -3, GETDATE()), 45, 320, 0, 42),
(2, 'Ashu', DATEADD(day, -2, GETDATE()), 120, 450, 0, 115),
(3, 'Rigon', DATEADD(day, -7, GETDATE()), 200, 680, 0, 195),
(1, 'Alex', DATEADD(day, -1, GETDATE()), 12, 320, 0, 10);
GO

-- =====================================================
-- VIEWS FOR REPORTING
-- =====================================================

-- View for overdue resources
CREATE VIEW vw_OverdueResources AS
SELECT 
    r.Id,
    r.Title,
    r.Author,
    br.BorrowerName,
    br.BorrowerEmail,
    br.BorrowDate,
    br.DueDate,
    DATEDIFF(day, br.DueDate, GETDATE()) AS DaysOverdue
FROM Resources r
INNER JOIN BorrowRecords br ON r.Id = br.ResourceId
WHERE br.IsReturned = 0 AND br.DueDate < GETDATE();
GO

-- View for resources by category
CREATE VIEW vw_ResourcesByCategory AS
SELECT 
    Type,
    CASE 
        WHEN Type = 0 THEN 'Book'
        WHEN Type = 1 THEN 'Journal'
        WHEN Type = 2 THEN 'Media'
        ELSE 'Unknown'
    END AS TypeName,
    COUNT(*) AS ResourceCount,
    AVG(Rating) AS AverageRating
FROM Resources
GROUP BY Type;
GO

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- Procedure to get library statistics
CREATE PROCEDURE sp_GetLibraryStatistics
AS
BEGIN
    SELECT 
        'Total Resources' AS Metric,
        COUNT(*) AS Value
    FROM Resources
    
    UNION ALL
    
    SELECT 
        'Available Resources' AS Metric,
        COUNT(*) AS Value
    FROM Resources
    WHERE IsAvailable = 1
    
    UNION ALL
    
    SELECT 
        'Currently Borrowed' AS Metric,
        COUNT(*) AS Value
    FROM BorrowRecords
    WHERE IsReturned = 0
    
    UNION ALL
    
    SELECT 
        'Overdue Items' AS Metric,
        COUNT(*) AS Value
    FROM BorrowRecords
    WHERE IsReturned = 0 AND DueDate < GETDATE()
    
    UNION ALL
    
    SELECT 
        'Active Reading Sessions' AS Metric,
        COUNT(*) AS Value
    FROM ReadingSessions
    WHERE IsCompleted = 0;
END
GO

-- =====================================================
-- SAMPLE QUERIES FOR TESTING
-- =====================================================

-- Test the data insertion
SELECT 'Database created successfully with ' + CAST(COUNT(*) AS varchar(10)) + ' resources' AS Status
FROM Resources;

-- Show overdue items
SELECT * FROM vw_OverdueResources;

-- Show library statistics
EXEC sp_GetLibraryStatistics;

PRINT 'Library Management System database created successfully!';
PRINT 'Database contains the same 19 resources as the application with sample borrowing data.';
GO