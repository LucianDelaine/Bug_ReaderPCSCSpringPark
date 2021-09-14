# Configuration de la cible

 * OS : Windows 10 Entreprise
 * Driver : 
   * Constructeur SpringCard
   * Date : 03/02/2021
   * Version : 13.42.12.344
 * Runtime :
   * [.Net FWK 4.7.2](https://dotnet.microsoft.com/download/dotnet-framework/net472)

# Projet de test lecteur PCSC

Ce projet a pour but de mettre en évidence un bug d'initialisation de lecteur PCSC au démarrage de l'OS. 
Pour cela, le résultat du projet produit un executable service windows à installer selon la procédure qui suit.

## Quelques informations 

 * Par défaut, le projet crée des fichiers de log dans le dossier `C:Temp`, le dossier est crée si absent. 
   * Si besoin, modifiez les valeurs comprises dans le fichier `SpringCardLog.xml`
 * Si vous souhaitez modifier des codes sources et reinstaller un service préalablement configuré
   * arrêtez celui-ci dans le gestionnaire des tâches
   * écrasez les fichiers préalablement configurés
   * relancez le service

## Installation

 1. Copier/Coller le résultat de la génération (contenu du dossier `generatedService`, autogénéré à la racine de la solution) à l'emplacement souhaité
 2. Ouvrir un interface de commande en administrateur à l'emplacement de l'executable.
 3. lancer la commande ci-dessous, en mettant à la place de ${PATH_Bug_ReaderPCSCService} la path complet de l'executable copié : 
 
 `sc create POC_service binpath= "${PATH_Bug_ReaderPCSCService}" start= auto`
 
> Dorénavant, un service Windows démarre en fond au démarrage de l'ordinateur et tente une connection PCSC.


