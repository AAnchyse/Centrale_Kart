Travello_Kart

Pr�sentation du projet:

D�velopper un jeu de course multijoueur arcade dans le style de Mario Kart avec des items pour attaquer les adversaires

Cahier des charges (� compl�ter au fur et � mesure):

Jeu de course de voiture multijoueur (local/en ligne).
Les joueurs peuvent attaquer les adversaires en se servant d'items al�atoires r�cup�r�s sur le terrains.

Liste d'items: -

Physique du kart (sondage):	- type arcade (Alex)
					- type r�aliste	
Circuit: 	- forme basique avec quelques tremplins et 				raccourcis
		- d�termination en temps r�el du classement des 			joueurs
		-impossibilit� de franchir les barri�res
		-3 tours
Kart: - design original ( li� � un univers particulier ou random?)

Univers:

Personnages:
		

Premi�re version du jeu:

Un premier prototype du jeu a �t� d�velopp�. Il servira de mat�riau de base au projet.


Les �l�ments suivants ont �t� d�velopp�s:

	-Physique �l�mentaire du kart (KartToutTerrain.cs):
		-Acc�l�ration/d�c�l�ration (forward force)
		-Tourner (torque)
		-D�rapage (torque + forward speed)
		-Suspensions (raycast)
		-Force emp�chant le kart de se retourner ( force at 		position)
		-Vitesse maximale
		-Forces frottement

	-Cam�ra du kart (CameraController):
		-Suit les mouvements du kart de fa�on fluide

	-Kart:
		-Mesh
		-Rigidbody
		-Box Collider

	-Terrain:
		-Terrain collider

El�ments � am�liorer:

La physique du kart de mani�re g�n�rale (suspensions, d�rapages, retournement, frottements en priorit�). S'inspirer d'�quations utilis�es pour simuler des voitures de course de type arcade ou plus de simulation ( � d�cider vers quoi on s'oriente).

Le terrain de base dans unity g�re mal les collisions et le kart passse souvent � travers, pratique pour prototyper mais il faudra designer nos propres terrains.

D�tacher les roues du mesh du kart afin d'avoir la sensation qu'elles touchent le sol ( les accrocher aux raycast lorsqu'ils touchent le sol)




		
