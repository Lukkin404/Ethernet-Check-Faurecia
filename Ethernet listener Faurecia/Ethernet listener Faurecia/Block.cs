namespace Ethernet_listener_Faurecia
{
    internal class Block
    {
        protected int id, nbDesk;
        protected Desk[] deskList;
        protected static string sourceFilePath = "../nbDesksPerBlock"; // chemin relatif du fichier source


        /* NAME : Block
         * IN   : Un entier (son id)
         * OUT  : X
         * VA   : Constructeur initialisant un nouveau bloc avec son id et son nombre de bureau grâce à la fonction Lecture(), qui permet ensuite d'en créer une liste
         */
        public Block(int id)
        {
            this.id = id + 1; // + 1 pour ajuster avec la liste commençant par 0
            nbDesk = Lecture(this.id);
            deskList = new Desk[nbDesk];
            for (int i = 0; i < nbDesk; i++) deskList[i] = new Desk(i, this.id); // Création des bureaux
        }


        /* NAME : Lecture
         * IN   : Un entier (le numéro du bloc à chercher dans le fichier source)
         * OUT  : X
         * VA   : Adapte l'entier entré en paramètre au format demandé (nombre en deux chiffres, même < à 10), puis recherche dans le fichier source ce nombre précédé du caractère '|' pour ensuite trouver plus loin son nombre de bureaux
         */
        public static int Lecture(int n)
        {
            string userChoice;
            if (n < 10) userChoice = "0" + n;
            else userChoice = n + "";

            string line;
            StreamReader lecture = new StreamReader(sourceFilePath);
            line = lecture.ReadLine(); // Copie le texte du fichier mis en argument de la méthode vers line
            lecture.Close(); // Fermeture pour libérer la mémoire

            int indexChoosenBlock = line.IndexOf("|" + userChoice);
            line = line.Substring(indexChoosenBlock + 4, 2); // Avancement vers l'endroit où est indiqué le nb de bureaux

            return Convert.ToInt32(line);
        }


        /* NAME : getId
         * IN   : X
         * OUT  : Un entier (l'id du Block en question)
         * VA   : Fontion de get classique, retournant l'id
         */
        public int getId(){ return id; }


        /* NAME : getNbEthernetConnection
         * IN   : X
         * OUT  : Un entier (le nombre de bureaux présents dans le Block en question)
         * VA   : Fontion de get classique, retournant le nombre de bureaux du bloc
         */
        public int getNbEthernetConnection(){ return nbDesk; }


        /* NAME : getDeskList
         * IN   : X
         * OUT  : Un Desk[] (la liste des bureaux présents dans le Block en question)
         * VA   : Fontion de get classique, retournant la liste des bureaux du bloc
         */
        public Desk[] getDeskList(){ return deskList; }
    }


}
