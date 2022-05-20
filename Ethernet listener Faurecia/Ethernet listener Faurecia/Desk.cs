namespace Ethernet_listener_Faurecia
{
    internal class Desk
    {
        protected string name;
        protected int id, idBlock, workingEthernet = 0; // à changer


        /* NAME : Desk
         * IN   : Deux entiers (son id et l'id du bloc dans lequel il se trouve)
         * OUT  : X
         * VA   : Constructeur initialisant un nouveau bureau avec son id, et l'id du bloc auquel il appartient
         */
        public Desk(int id, int idBlock)
        {
            this.id = id + 1; // + 1 pour ajuster avec la liste commençant par 0
            this.idBlock = idBlock;
            name = "Q";
            if (this.idBlock<10) name += "0" + Convert.ToString(this.idBlock);
            else name += Convert.ToString(this.idBlock);
            name += "-";
            if (this.id<10) name += "0" + Convert.ToString(this.id);
            else name += Convert.ToString(this.id);
        }


        /* NAME : getWorkingEthernetState
         * IN   : X
         * OUT  : Un string (état de workingEthernet interprété)
         * VA   : Retourne l'état de l'attribut workingEthernet du Desk en question au format textuel ("pas teste", "OK" ou "NOK")
         */
        public string getWorkingEthernetState()
        {
            if (workingEthernet == 0) return "pas teste";
            else if (workingEthernet == 1) return "OK";
            else if (workingEthernet == 2) return "NOK";
            else return "";
        }


        /* NAME : setWorkingEthernet
         * IN   : Un entier (0 si pas testé, 1 si fonctionnel, 2 si non-fonctionnel)
         * OUT  : X
         * VA   : Fontion de set modifiant l'attribut workingEthernet avec le paramètre IN, après avoir vérifié si ce dernier était conforme au format attendu (de 0 à 3)
         */
        public void setWorkingEthernet(int state)
        {
            if(state >= 0 && state < 4) workingEthernet = state;
        }


        /* NAME : ToString
         * IN   : X
         * OUT  : Un string (affichage des informations du desk en question)
         * VA   : Fontion de ToString classqique, gérant l'affichage des objets Desk de la classe
         */
        public new string ToString()
        {
            return string.Format("\nLa prise ethernet du bureau {0} est en etat '{1}'\n\n",
                /*this.idBlock < 10 ? "0" + (this.idBlock) : this.idBlock,
                this.id < 10 ? "0" + (this.id): this.id,*/
                this.name,
                this.getWorkingEthernetState()); // -1 pour ajuster avec la liste commençant par 0
        }


        /* NAME : getName
         * IN   : X
         * OUT  : Une chaîne de caractères (le nom du Desk en question)
         * VA   : Fontion de get classique, retournant le nom
         */
        public string getName() { return name; }


        /* NAME : getId
         * IN   : X
         * OUT  : Un entier (l'id du Desk en question)
         * VA   : Fontion de get classique, retournant l'id
         */
        public int getId(){ return id; }
    }
}
