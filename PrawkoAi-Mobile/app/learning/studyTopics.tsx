import { MaterialIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import React from "react";
import {
  Platform,
  ScrollView,
  StatusBar,
  Text,
  TextInput,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Footer from "../components/footer";
import categoryMap from "../utils/categoryMap";

const userProgress = [
  { id: "MandatoryAndWarningSigns", completed: 7, total: 45 },
  { id: "UncontrolledAndPriorityIntersections", completed: 15, total: 45 },
];

const sections = [
  {
    title: "Znaki i Oznakowanie",
    part: 1,
    type: "list",
    ids: ["MandatoryAndWarningSigns", "InformationAndRoadMarkings"],
  },
  {
    title: "Skrzyżowania i Ruch",
    part: 2,
    type: "grid",
    ids: [
      "UncontrolledAndPriorityIntersections",
      "SignalizedIntersectionsAndPedestrians",
      "ManoeuvresAndPositioning",
      "OvertakingAndPassing",
    ],
  },
  {
    title: "Pojazd i Bezpieczeństwo",
    part: 3,
    type: "compact",
    ids: [
      "VehicleLightsAndSignals",
      "SocialBehaviourAndSecuring",
      "RailCrossingsAndPublicTransport",
      "EmergencyAndFitnessToDrive",
      "SpeedAndBrakingDistances",
      "SafetyFirstAidAndDocuments",
    ],
  },
];

export default function StudyTopicsScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const paddingTop =
    Platform.OS === "android"
      ? insets.top > 0
        ? insets.top
        : StatusBar.currentHeight || 24
      : insets.top;

  const getProgress = (id: string) =>
    userProgress.find((p) => p.id === id) || { completed: 0, total: 45 };

  return (
    <View className="flex-1 bg-[#f6f6f8] dark:bg-[#111621]">
      <StatusBar
        barStyle="dark-content"
        translucent
        backgroundColor="transparent"
      />

      <View
        className="bg-white/95 dark:bg-[#1a1f2e] border-b border-blue-900/10 shadow-sm z-50"
        style={{ paddingTop }}
      >
        <View
          style={{
            height: 56,
            flexDirection: "row",
            alignItems: "center",
            paddingHorizontal: 12,
          }}
        >
          <TouchableOpacity
            onPress={() => router.back()}
            className="p-2"
            hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
          >
            <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
          </TouchableOpacity>
          <Text className="text-xl font-bold ml-2 text-black">
            Tematy Nauki
          </Text>
        </View>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{
          paddingHorizontal: 16,
          paddingTop: 16,
          paddingBottom: Platform.OS === "ios" ? 100 : 20,
        }}
        showsVerticalScrollIndicator={false}
      >
        <View className="mb-6 flex-row items-center bg-white dark:bg-slate-800 rounded-2xl px-4 shadow-sm h-14">
          <MaterialIcons name="search" size={20} color="#475569" />
          <TextInput
            className="flex-1 ml-3 text-slate-800 dark:text-white"
            placeholder="Szukaj tematów..."
            placeholderTextColor="#94a3b8"
          />
        </View>

        <View className="mb-10 bg-[#1544b2] rounded-[32px] p-6 shadow-lg relative overflow-hidden">
          <View className="z-10 relative">
            <View className="bg-white/20 self-start px-3 py-1 rounded-full">
              <Text className="text-[10px] font-bold text-white uppercase tracking-widest">
                Codzienna porada
              </Text>
            </View>
            <Text className="text-2xl font-bold text-white mt-3">
              Opanuj skrzyżowania
            </Text>
            <Text className="text-blue-100 text-sm mt-1 max-w-[200px]">
              Skup się dzisiaj na zasadach pierwszeństwa.
            </Text>
            <TouchableOpacity className="mt-4 bg-white self-start px-6 py-2.5 rounded-xl">
              <Text className="text-[#1544b2] font-bold">Rozpocznij naukę</Text>
            </TouchableOpacity>
          </View>
          <MaterialIcons
            name="psychology"
            size={160}
            color="white"
            style={{
              position: "absolute",
              right: -30,
              bottom: -30,
              opacity: 0.1,
            }}
          />
        </View>

        {sections.map((section, sIdx) => (
          <View key={sIdx} className="mb-10">
            <View className="flex-row justify-between items-center mb-4 px-2">
              <Text className="font-bold text-lg text-slate-900 dark:text-white">
                {section.title}
              </Text>
              <View className="bg-[#e0e7ff] px-3 py-1 rounded-full">
                <Text className="text-[10px] font-bold text-[#1544b2] uppercase">
                  Część {section.part}
                </Text>
              </View>
            </View>

            {section.type === "list" && (
              <View>
                {section.ids.map((id) => {
                  const cat = categoryMap[id];
                  const prog = getProgress(id);
                  return (
                    <TouchableOpacity
                      key={id}
                      className="bg-white dark:bg-slate-800 p-5 rounded-2xl shadow-sm border border-blue-500/10 flex-row mb-4"
                    >
                      <View className="w-12 h-12 rounded-xl bg-blue-50 dark:bg-blue-900/20 items-center justify-center mr-4">
                        <MaterialIcons
                          name={cat.icon as any}
                          size={24}
                          color="#1544b2"
                        />
                      </View>
                      <View className="flex-1">
                        <Text className="font-bold text-slate-800 dark:text-white">
                          {cat.name}
                        </Text>
                        <Text className="text-xs text-slate-500 mb-4">
                          Podstawy i zasady
                        </Text>
                        <View className="flex-row items-center">
                          <View className="flex-1 h-2 bg-slate-100 dark:bg-slate-700 rounded-full overflow-hidden mr-3">
                            <View
                              className="h-full bg-[#1544b2]"
                              style={{
                                width: `${(prog.completed / prog.total) * 100}%`,
                              }}
                            />
                          </View>
                          <Text className="text-[10px] font-bold text-slate-400">
                            {prog.completed}/{prog.total}
                          </Text>
                        </View>
                      </View>
                    </TouchableOpacity>
                  );
                })}
              </View>
            )}

            {section.type === "grid" && (
              <View
                className="flex-row flex-wrap justify-between"
                style={{ gap: 12 }}
              >
                {section.ids.map((id) => {
                  const cat = categoryMap[id];
                  const prog = getProgress(id);
                  const gridColors: any = {
                    UncontrolledAndPriorityIntersections: {
                      bg: "bg-orange-50",
                      text: "#ea580c",
                      icon: "priority-high",
                    },
                    SignalizedIntersectionsAndPedestrians: {
                      bg: "bg-emerald-50",
                      text: "#059669",
                      icon: "traffic",
                    },
                    ManoeuvresAndPositioning: {
                      bg: "bg-indigo-50",
                      text: "#4f46e5",
                      icon: "directions-car",
                    },
                    OvertakingAndPassing: {
                      bg: "bg-blue-50",
                      text: "#2563eb",
                      icon: "move-up",
                    },
                  };
                  const style = gridColors[id] || {
                    bg: "bg-blue-50",
                    text: "#1544b2",
                    icon: "help",
                  };

                  return (
                    <TouchableOpacity
                      key={id}
                      className="bg-white dark:bg-slate-800 p-4 rounded-2xl shadow-sm border border-blue-500/10 mb-1 flex-1 min-w-[45%]"
                    >
                      <View
                        className={`w-10 h-10 rounded-lg ${style.bg} items-center justify-center mb-3`}
                      >
                        <MaterialIcons
                          name={style.icon as any}
                          size={20}
                          color={style.text}
                        />
                      </View>
                      {/* Flex name container bez sztywnej wysokości */}
                      <View style={{ minHeight: 42, justifyContent: "center" }}>
                        <Text className="font-bold text-slate-800 dark:text-white text-[11px] leading-4">
                          {cat.name}
                        </Text>
                      </View>
                      <View className="flex-row justify-between items-center mt-2">
                        <Text className="text-[9px] font-bold text-[#1544b2] uppercase">
                          Status
                        </Text>
                        <Text className="text-[10px] font-bold text-slate-400">
                          {prog.completed}/{prog.total}
                        </Text>
                      </View>
                    </TouchableOpacity>
                  );
                })}
              </View>
            )}

            {section.type === "compact" && (
              <View className="bg-white dark:bg-slate-800 rounded-2xl shadow-sm border border-blue-500/10 overflow-hidden">
                {section.ids.map((id, idx) => {
                  const cat = categoryMap[id];
                  const prog = getProgress(id);
                  return (
                    <TouchableOpacity
                      key={id}
                      className={`p-4 flex-row items-center border-b border-slate-50 dark:border-slate-700 ${idx === section.ids.length - 1 ? "border-b-0" : ""}`}
                    >
                      <MaterialIcons
                        name={cat.icon as any}
                        size={20}
                        color="#94a3b8"
                      />
                      <Text className="flex-1 ml-4 text-sm font-medium text-slate-800 dark:text-white">
                        {cat.name}
                      </Text>
                      <Text className="text-[10px] font-bold text-slate-400 mr-2">
                        {prog.completed}/{prog.total}
                      </Text>
                      <MaterialIcons
                        name="chevron-right"
                        size={20}
                        color="#cbd5e1"
                      />
                    </TouchableOpacity>
                  );
                })}
              </View>
            )}
          </View>
        ))}
      </ScrollView>

      {/* Footer siedzi grzecznie pod ScrollView */}
      <Footer />
    </View>
  );
}
