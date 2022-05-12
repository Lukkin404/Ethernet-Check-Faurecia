using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Ethernet_listener_Faurecia;

namespace FaureciaEthernetListener
{
    internal class Program
    {
        protected static int nbBlock, chosenBlock, chosenDesk, checkOrEditMode /* 1 pour check, 2 pour edit */;
        protected static string sourceFilePath = "../../../../../nbDesksPerBlock"/* chemin relatif du fichier source */,
            xlsxFilePath = "../../../../../test.xlsx"/* chemin relatif du fichier Excel */, userInput;
        protected static bool goBack = false;
        protected static char cancelChar = 'q';


        static void Main(string[] args)
        {
            newSpreadsheet();

            nbBlock = blockNumberDetection();

            // ****     Création des blocks (en liste)  **** \\
            Block[] blockList = new Block[nbBlock];
            for (int i = 0; i < nbBlock; i++) blockList[i] = new Block(i);


            // ****     Boucle du programme principal   **** \\
            while (true) {
                goBack = false;
                checkOrEditMode = CheckOrEdit();
                if (goBack == true) break;

                while (true) {
                    goBack = false;
                    Block selectedBlock = BlockSelection(blockList);
                    if (goBack == true) break;

                    while (true) {
                        goBack = false;

                        if (checkOrEditMode == 1) // si choix check
                        {
                            Desk displayDesk = DeskSelection(selectedBlock);
                            if (goBack != true)
                                Console.WriteLine(displayDesk.ToString());
                            else break;
                        }

                        else if (checkOrEditMode == 2) // si choix edit
                        {
                            Desk displayDesk = DeskSelection(selectedBlock);
                            if (goBack!= true)
                                   DeskStateEdit(displayDesk);
                            else break;
                        }
                    }
                }
            }
                Console.WriteLine("\nAu revoir, a bientot !\n");
            System.Environment.Exit(0);
        }


        /* NAME : blockNumberDetection
         * IN   : X
         * OUT  : Un entier (le nombre de blocs)
         * VA   : Détecte le nombre de blocs totaux inscrits dans le fichier source, en comptant un caractère de référence ('|')
         */
        private static int blockNumberDetection()
        {
            int nbBlock;
            string line;
            char reference = '|'; // caractère avant chaque bloc dans la liste, permettant de les compter

            
            try {
                StreamReader lecture = new StreamReader(sourceFilePath);
                line = lecture.ReadLine(); // Copie le texte du fichier mis en argument de la méthode vers line
                lecture.Close(); // Fermeture pour libérer la mémoire

                nbBlock = line.Count(n => n == reference); // compte le nombre d'occurence de reference dans line
                return nbBlock;
            } catch {
                Console.WriteLine("Erreur de lecture du fichier source");
                System.Environment.Exit(1);
                return 1;
            }
        }


        /* NAME : CheckOrEdit
         * IN   : X
         * OUT  : Un entier (le choix de l'utilisateur)
         * VA   : Demande à l'utilisateur s'il veut consulter ou mofifier les infos (1 pour check, 2 pour modifier, q pour quitter)
         */
        private static int CheckOrEdit()
        {
            userInput = "0";
            Console.Write("\nVoulez-vous consulter des valeurs [1] ou en modifier [2] ? : ");
            do {
                try { userInput = Console.ReadLine(); }
                catch { Exception e; }
                if (userInput == Convert.ToString(cancelChar)) goBack = true; // quitte l'application
            } while (userInput != "1" && userInput != "2" && goBack != true);
            if (userInput == "1" || userInput == "2") return Convert.ToInt32(userInput);
            else return 0; // en cas de retour en arrière
        }


        /* NAME : BlockSelection
         * IN   : Block[] (une liste de Block)
         * OUT  : Un Block (le bloc sélectionné par l'utilisateur)
         * VA   : Demande à l'utilisateur dans quel bloc de l'open space il veut opérer (q pour quitter)
         */
        private static Block BlockSelection(Block[] blockList)
        {
            Console.Write("\nQuel bloc de l'open space ? : ");
            do {
                try {
                    userInput = Console.ReadLine();
                    if (userInput == Convert.ToString(cancelChar)) goBack = true;
                    else chosenBlock = Convert.ToInt32(userInput);
                }
                catch { Exception e; }
            } while ((chosenBlock <= 0 || chosenBlock > nbBlock) && goBack != true);
            if (chosenBlock > 0 && chosenBlock <= nbBlock)
            {
                Console.Write("Le bloc selectionne est le numero {0}\n", chosenBlock);
                return blockList[chosenBlock - 1];
            }
            else return null; // en cas de retour en arrière
        }


        /* NAME : DeskSelection
         * IN   : Block
         * OUT  : Un Desk (le bureau sélectionné par l'utilisateur)
         * VA   : Demande à l'utilisateur quel bureau du bloc il veut sélectionner (q pour quitter)
         */
        private static Desk DeskSelection(Block BlockSelection)
        {
            Console.Write("\nBureau numero : ");
            do {
                do try
                    {
                        chosenDesk = -1;
                        userInput = Console.ReadLine();
                        if (userInput == Convert.ToString(cancelChar)) goBack = true;
                        else chosenDesk = Convert.ToInt32(userInput);
                    }
                    catch { Exception e; }
                while ((chosenDesk <= 0 || chosenDesk > BlockSelection.getNbEthernetConnection()) && goBack != true);
            } while (chosenBlock == -1); // Si la valeur reste à -1, alors c'est que la valeur entrée ensuite est incorrecte, donc une nouvelle entrée est demandée
            Desk[] editedList = BlockSelection.getDeskList();

            if (chosenDesk > 0 && chosenDesk <= BlockSelection.getNbEthernetConnection())
                return editedList[chosenDesk - 1];
            else return null; // en cas de retour en arrière
        }


        /* NAME : SpacebarPressed
         * IN   : X
         * OUT  : Un bool (état de la barre espace)
         * VA   : Retourne 'true' si la barre espace est appuyée
         */
        private static bool SpacebarPressed()
        {
            if(Console.ReadKey(true).Key == ConsoleKey.Spacebar);
            return true;
        }


        private static void newSpreadsheet()
        {
            using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Open(xlsxFilePath,true))
            {
                // Add a WorksheetPart.  
                WorksheetPart newWorksheetPart = spreadsheet.WorkbookPart.AddNewPart<WorksheetPart>();
                newWorksheetPart.Worksheet = new Worksheet(new SheetData());

                // Create Sheets object.  
                Sheets sheets = spreadsheet.WorkbookPart.Workbook.GetFirstChild<Sheets>();
                string relationshipId = spreadsheet.WorkbookPart.GetIdOfPart(newWorksheetPart);

                // Create a unique ID for the new worksheet.  
                uint sheetId = 1;
                if (sheets.Elements<Sheet>().Count() > 0)
                {
                    sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
                }

                // Give the new worksheet a name.  
                string sheetName = "mySheet" + sheetId;

                // Append the new worksheet and associate it with the workbook.  
                Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
                sheets.Append(sheet);
            }

        }


        /* NAME : DeskStateEdit
         * IN   : Un Desk (le bureau sélectionné par l'utilisateur)
         * OUT  : X
         * VA   : Vérifie si l'ordinateur est connecté à un réseau : si oui il l'écrit dans un fichier .ods ; sinon, il attend que l'utilisateur appuie sur la barre espace pour l'écrire dans le fichier .ods
         */
        private static void DeskStateEdit(Desk desk)
        {
            // Console.Write("\nAppuyez sur espace s'il n'y a pas de connexion internet.\n");

            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                desk.setWorkingEthernet(2);
            }
            
            else if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                desk.setWorkingEthernet(1);
            }
        }
    }
}

/*
 * à faire :
 *  -> écriture excel
 *  -  meilleure détection de taille de bloc (peu importe la taille du nb)
 */