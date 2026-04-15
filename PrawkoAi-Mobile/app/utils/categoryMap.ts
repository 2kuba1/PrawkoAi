const categoryMap: Record<string, { name: string; icon: string }> = {
  SafetyFirstAidAndDocuments: {
    name: "Bezpieczeństwo i Dokumenty",
    icon: "medical-services",
  },
  OvertakingAndPassing: {
    name: "Wyprzedzanie i Mijanie",
    icon: "compare-arrows",
  },
  SocialBehaviourAndSecuring: {
    name: "Zachowanie i Zabezpieczenie",
    icon: "groups",
  },
  SpeedAndBrakingDistances: { name: "Prędkość i Hamowanie", icon: "speed" },
  EmergencyAndFitnessToDrive: {
    name: "Sytuacje Awaryjne i Zdrowie",
    icon: "psychology",
  },
  ManoeuvresAndPositioning: {
    name: "Manewry i Pozycjonowanie",
    icon: "directions-car",
  },
  MandatoryAndWarningSigns: {
    name: "Znaki Nakazu i Ostrzegawcze",
    icon: "warning",
  },
  SignalizedIntersectionsAndPedestrians: {
    name: "Sygnalizacja i Pieszy",
    icon: "traffic",
  },
  UncontrolledAndPriorityIntersections: {
    name: "Skrzyżowania i Pierwszeństwo",
    icon: "alt-route",
  },
  RailCrossingsAndPublicTransport: {
    name: "Przejazdy Kolejowe i Komunikacja",
    icon: "train",
  },
  VehicleLightsAndSignals: { name: "Światła i Sygnały", icon: "lightbulb" },
  InformationAndRoadMarkings: {
    name: "Informacja i Znaki Poziome",
    icon: "map",
  },
};

export default categoryMap;
