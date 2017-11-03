1. NLSY79 Gen 1 & 2: refreshing w/ new survey releases
2. Modifying architecture so:
    * it's not dependent on a remote dedicated SQL Server database
    * anyone can theoretically add variables by modifying the CSV metadata files
    * 79 & 97 metedata files are managed independently
    * maybe it can better handle 97's emphasis on rosters
3. 97
    * strong emphasis on rosters
    * tracks history of residents & non-residents -even those not officially participants
    * I still haven't found the variables that lead to the subject ID (similar to `R0000150` in 79 Gen1).  I'll contact them if I don't find it in the next week.
4. Products
    * Links (97 & 79)
    * rosters in the normalized form (in the database sense of 'normalized')
    * I'd like most variables used in the analyses to be available through this system (and ideally in the NlsyLinks package too)
5. Important/representative files in the pipeline
    * `data-public/metadata/tables/item_79.csv`
    * `data-public/metadata/tables/variable_79.csv`
    * `data-public/metadata/tables/roster_assignment_79_gen1.csv`
    * `BaseAssembly/Response.cs`    
    * `BaseAssembly/BabyDaddy.cs`  
    * `BaseAssembly/RelatedValues.cs`
    * `BaseAssembly/Assign/RGen1Pass2.cs`
