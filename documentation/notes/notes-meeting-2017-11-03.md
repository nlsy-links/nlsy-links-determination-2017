1. NLSY79 Gen 1 & 2: refreshing w/ new survey releases
2. Modifying architecture so:
    * it's not dependent on a remote dedicated SQL Server database
    * anyone can theoretically add variables by modifying the CSV metadata files
    * 79 & 97 metadata files are managed independently
    * maybe it can better handle 97's emphasis on rosters
3. 97
    * strong emphasis on rosters
    * tracks history of residents & non-residents -even those not officially participants
    * I still haven't found the variables that lead to the subject ID (similar to `R0000150` in 79 Gen1).  I'll contact them if I don't find it in the next week.
4. Products
    * Pairwise Links (97 & 79)
    * rosters in the normalized form (in the database sense of 'normalized')
    * I'd like most variables used in the analyses to be available through this system (and ideally in the NlsyLinks package too)
5. Important/representative files in the pipeline
    * `data-public/metadata/tables/item_79.csv`
    * `data-public/metadata/tables/variable_79.csv`
    * `data-public/metadata/tables/roster_assignment_79_gen1.csv`
    * `Base79/Response.cs`    
    * `Base79/BabyDaddy.cs`  
    * `Base79/RelatedValues.cs`
    * `Base79/Assign/RGen1Pass2.cs`
6. Forums
    * Showcase articles & products
    * Bridges different articles & projects (which may disqualify GitHub as the entry point)
    * Q&A with the public
    * supports blogs/announcements
    * btw, it probably makes sense to create a new GitHub organization
7. Products
    * Articles
        1. NlsyLinks description in JSS
        1. ROC used in determination of links
    * Datasets
        1. *Pairwise kinship links*: one row per pair (this is the normal set of links)
        1. *Survival*: one row per person
        1. *Survey Year*: one row per person-year
        1. *Outcomes*:  one row per person
        1. *97 Roster(s)*: one row per family member
        1. *Pair distance*: one row per pair per year  (See issue [#31](https://github.com/LiveOak/nlsy-links-determination-2017/issues/31))

        > Create a dataset with one row per relationship/pair per year.
        >
        > See [Y0003300](https://www.nlsinfo.org/investigator/pages/search.jsp#Y0003301) and the rest of the 'Q2' 79Gen2 variables.
        >                
        > Variables
        > 1. year
        > 1. S1 Tag
        > 1. S2 Tag
        > 1. S1 Age (at interview)
        > 1. S2 Age (at interview)
        > 1. Proportion of Year S1 & S2 lived together
        > 1. Cumulative Years Together count
        > 1. S1 Q (= S1 Age / Cumulative Years Together count)
        > 1. S2 Q (= S2 Age / Cumulative Years Together count)
        >
        > Other plumbing variables
        > 1. RelationshipPath
        > 1. S1 Generation
        > 1. S2  Generation
        > 1. Is survey year (b/c we have a row for every year, not every survey year)
        >
        > Real-world
        > 1. the participants aren't surveyed on the same day necessarily, to the proportion & cumulative versions conceivably need two versions (one for each subject's interview date)
        >
        > Example Scenario
        > * Subjects 101 & 102 are half sibs, both born to Gen 1 mom (id: 1) and lived w/ mom all their lives
        > * In 2000
        > * 101 is 7 y.o.
        > * 102 is 2 y.o.
        >
        > Discussed at the Nov meeting
        
