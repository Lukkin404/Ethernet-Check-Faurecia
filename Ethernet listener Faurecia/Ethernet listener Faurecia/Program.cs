using Ethernet_listener_Faurecia;

namespace FaureciaEthernetListener
{
    internal class Program
    {
        protected static int nbBlock, chosenBlock, chosenDesk, checkOrEditMode /* 1 pour check, 2 pour edit */;
        protected static string sourceFilePath = "../../../../../nbDesksPerBlock"/* chemin relatif du fichier source */,
            csvFilePath = "../../../../../tableauEtatEthernet-" + DateTime.Now.ToString("yyyyMMdd-HHmm") + ".csv"/* chemin relatif du fichier Excel */, userInput;
        protected static bool goBack = false;
        protected static char cancelChar = 'q';


        static void Main(string[] args)
        {
            nbBlock = blockNumberDetection();

            // ****     Création des blocks (en liste)  **** \\
            Block[] blockList = new Block[nbBlock];
            for (int i = 0; i < nbBlock; i++) blockList[i] = new Block(i);

            StreamWriter sw = InitCSVFile(blockList);

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
                            if (goBack != true)
                            {
                                DeskStateEdit(displayDesk, sw);
                                InitCSVFile(blockList);
                            }
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
                return 1;
                System.Environment.Exit(1);
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
                } catch { Exception e; }
                while ((chosenDesk <= 0 || chosenDesk > BlockSelection.getNbEthernetConnection()) && goBack != true);
            } while (chosenBlock == -1); // Si la valeur reste à -1, alors c'est que la valeur entrée ensuite est incorrecte, donc une nouvelle entrée est demandée
            Desk[] editedList = BlockSelection.getDeskList();

            if (chosenDesk > 0 && chosenDesk <= BlockSelection.getNbEthernetConnection())
                return editedList[chosenDesk - 1];
            else return null; // en cas de retour en arrière
        }


        /* NAME : InitCSVFile
         * IN   : Block[] (une liste de Block)
         * OUT  : Un StreamWriter (pour écrire dans un fichier)
         * VA   : Imprime dans le fichier .csv horodaté l'état actuel de la liste des bureaux de chaque bloc sous forme de tableur interprétable (colonnes séparées par ',')
         */
        private static StreamWriter? InitCSVFile(Block[] blockList)
        {
            try{
                StreamWriter ecriture = new StreamWriter(csvFilePath);
                ecriture.Write("ID Bureau,Etat\n");
                foreach (Block block in blockList)
                {
                    foreach (Desk desk in block.getDeskList())
                    {
                        ecriture.Write("{0},{1}\n",
                            desk.getName(),
                            desk.getWorkingEthernetState());
                    }
                }
                ecriture.Close();
                return ecriture;
            } catch {
                Console.WriteLine("Erreur d'ecriture");
                return null;
            }
        }


        /* NAME : DeskStateEdit
         * IN   : Un Desk (le bureau sélectionné par l'utilisateur)
         * OUT  : X
         * VA   : Vérifie si l'ordinateur est connecté à un réseau : si oui il l'écrit dans un fichier .ods ; sinon, il attend que l'utilisateur appuie sur la barre espace pour l'écrire dans le fichier .ods
         */
        private static void DeskStateEdit(Desk desk, StreamWriter sw)
        {
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                desk.setWorkingEthernet(2);
            }
            
            else if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
                desk.setWorkingEthernet(1);
            }

            //EditCSVFile(desk);
        }
    }
}