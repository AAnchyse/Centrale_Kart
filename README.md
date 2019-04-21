Travello_Kart

Présentation du projet:

Développer un jeu de course multijoueur arcade dans le style de Mario Kart avec des items pour attaquer les adversaires

Cahier des charges (à compléter au fur et à mesure):

Jeu de course de voiture multijoueur (local/en ligne).
Les joueurs peuvent attaquer les adversaires en se servant d'items aléatoires récupérés sur le terrains.

Liste d'items: -

Physique du kart (sondage):	- type arcade (Alex)
				- type réaliste	
Circuit: 	- forme basique avec quelques tremplins et raccourcis
		- détermination en temps réel du classement des joueurs
		-impossibilité de franchir les barrières
		-3 tours
Kart: - design original ( lié à un univers particulier ou random?)

Univers:

Personnages:
		

Première version du jeu:

Un premier prototype du jeu a été développé. Il servira de matériau de base au projet.
https://www.youtube.com/watch?v=x_MaWFpt2W0

Les éléments suivants ont été développés:

	-Physique élémentaire du kart (KartToutTerrain.cs):
		-Accélération/décélération (forward force)
		-Tourner (torque)
		-Dérapage (torque + forward speed)
		-Suspensions (raycast)
		-Force empêchant le kart de se retourner ( force at position)
		-Vitesse maximale
		-Forces frottement

	-Caméra du kart (CameraController):
		-Suit les mouvements du kart de façon fluide

	-Kart:
		-Mesh
		-Rigidbody
		-Box Collider

	-Terrain:
		-Terrain collider

Eléments à améliorer:

La physique du kart de manière générale (suspensions, dérapages, retournement, frottements en priorité). S'inspirer d'équations utilisées pour simuler des voitures de course de type arcade ou plus de simulation ( à décider vers quoi on s'oriente).

Le terrain de base dans unity gère mal les collisions et le kart passse souvent à travers, pratique pour prototyper mais il faudra designer nos propres terrains.

Détacher les roues du mesh du kart afin d'avoir la sensation qu'elles touchent le sol ( les accrocher aux raycast lorsqu'ils touchent le sol)




		
