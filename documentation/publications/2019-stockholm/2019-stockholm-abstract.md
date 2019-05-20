Abstract
------------------------

**Original and Replication Research with the NLSY79 and NLSY97 Kinship Links**

The 24,000 subjects in the NLSY79 and their children have supported [over 75 biometric publications](http://liveoak.github.io/NlsyLinks/research-publications.html) since 1993, thanks to four distinct funding cycles to estimate the Rs of the 42,000 familial pairs.  Our current project updates both generations of the NLSY79, as well as introduces an additional 2,500 kinship links for the 9,000 subjects in the newer NLSY97 sample.

We discuss how these intra-individual longitudinal datasets can quickly contribute to different types of behavior genetic research labs.  
1. small labs (who do not posses the time or funding to establish their own life course dataset) can immediately download 37 years of detailed history of the NLSY79 extended families and publish their findings.  
2. maturing labs (who are planning which instruments to introduce in their expanding twin study) can run similar analyses of this nationally-representative sample to evaluate and justify their proposals.  
3. distinguished labs (who manage their own productive twin study) can leverage the thousands of economic, cognitive, education, and health variables to replicate and further support their substantive findings.

We have derived and released additional datasets the can actuate non-BG investigations, and are available in all modeling software.  R users may further benefit from the [NlsyLinks](http://liveoak.github.io/NlsyLinks/) package, which allows researchers to estimate basic biometric models with ten lines of code.

Loose Notes
------------------------

* Joe's suggestions (2019-05-20)
    * There's value of seeing the 'http://liveoak.github.io/NlsyLinks/' site w/ screenshots.  Then go into 'Research', and demonstrate the access to the datasets (for those not using the R package).  If time, show the existence of the vignettes & documentation examples.
    * emphasize that we have 97.  It's value migth be accelerated, when combined with 79.
    * we're kinda done with the kinship links, except for updates.
    * but there are other features of the package/datasets beyond the kinship link
    * for example, all of Gen1 passed age 50, which complete the `H50` health battery (which follows the `H40` battery).
    * the next big age marker is "the last 79Gen2 becomes age 15" (ie, crosses from 'child' to 'young adult'). "In a few years, every single Gen participant will be in the 'young adult' category.  The selection bias (towards chidren born to younger moms) is rapidly disappearing."

* primarily about 97 + updates of 79

* mention the NlsyLinks package

* links have two purposes
    1. initiate original research for those who don't have access to a biometric dataset
    2. validate existing research from conventional BG datasets (reference the IALSA paper).

        * Quick/cheap way of validating

* NLSY has thousands of variables across different domains.  There is a good chance your team's existing substantive expertise will be supported by NLSY variables.

* Benefits
    1. nationally-representative sample
    1. longitudinal tracking an individual.  
    1. individuals are nested within family across 2+ generations

* The dataset is supported by any statistical software, but the R package allows you to write full biometrical models within 10 lines of code.

* Additional datasets that aren't biometric

The NLSY is a collection of three samples tracking individuals and their offspring for up to forty years.  Using explicit and implicit responses, we estimate the proportion of shared genes in each familial relationship. An example of an explicit item is "Do you share the same biological father?" An example of an implicit item is "How far away does your biological father live?" The dataset contains 30,000 people and 85,000 links.

(Data collection 79 began in 1979 and has occurred every other year since, and 97 began in 97 and has been collected every other year since 1997. NLSY 79 gen1 and gen2 NLSY 97 gen1). The 97 was kind of a disappointment because most of the links are 0.5 or 0, in contrast the 79 contains a lot of half-siblings (0.25) which helped improve model estimation.  The 79 also has cross-generational links.  Conventional family with 2 gen1 sisters each with 2 gen 2 offspring, six choose 2 is 15, supports 15 links, but since the 97 has only 1 generation, 2 gen1 sisters produce only 1 link.  The NLSY links is an R package that serves 2-3 purposes.  with 3 components.  The first is it contains biometrical datasets 2) it contains functions that prepare conventional datasets for biometrical analysis, and 3) it contains   documentation and vignettes to help those without much experience with biometrical analysis or the NLSY.


* Part of our grant funds people's time to support other research teams' NLSY questions, especially related to BG.

* A manageable master's thesis could be replicating your team's original research (with your twin sample) with the broader NLSY subjects and variables.

* List the groomed variables we already provide.
