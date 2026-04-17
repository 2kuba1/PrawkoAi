import { MaterialIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import React, { useMemo } from "react";
import {
  Dimensions,
  Platform,
  ScrollView,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import Svg, { Path } from "react-native-svg";
import Footer from "../components/footer";

const { width: SCREEN_WIDTH } = Dimensions.get("window");

const NODE_HEIGHT = 160;
const OFFSET_X = 55;

export default function ScalableExamPathScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();

  const sets = [
    { id: 1, title: "Zestaw 1", score: 100, status: "completed" },
    { id: 2, title: "Zestaw 2", score: 85, status: "completed" },
    { id: 3, title: "Zestaw 3", score: 0, status: "active" },
    { id: 4, title: "Zestaw 4", score: 0, status: "locked" },
    { id: 5, title: "Zestaw 5", score: 0, status: "locked" },
    { id: 6, title: "Zestaw 6", score: 0, status: "locked" },
  ];

  const paddingTop =
    Platform.OS === "android" ? (insets.top > 0 ? insets.top : 24) : insets.top;

  const totalNodes = sets.length + 1;
  const svgPathData = useMemo(() => {
    let d = `M 200 0`;
    for (let i = 0; i < totalNodes; i++) {
      const x = i % 2 === 0 ? 260 : 140;
      const y = (i + 1) * NODE_HEIGHT;
      d += ` Q ${x} ${y - NODE_HEIGHT / 2}, 200 ${y}`;
    }
    return d;
  }, [totalNodes]);

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
            onPress={() =>
              router.canGoBack() ? router.back() : router.replace("/dashboard")
            }
            className="p-2"
            hitSlop={{ top: 10, bottom: 10, left: 10, right: 10 }}
          >
            <MaterialIcons name="arrow-back" size={24} color="#1544b2" />
          </TouchableOpacity>
          <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
            Ścieżka nauki
          </Text>
        </View>
      </View>

      <ScrollView
        className="flex-1"
        contentContainerStyle={{
          paddingTop: 40,
          paddingBottom: 80,
        }}
        showsVerticalScrollIndicator={false}
      >
        <View
          className="absolute top-0 left-0 right-0 bottom-0 items-center"
          pointerEvents="none"
        >
          <Svg width={SCREEN_WIDTH} height={totalNodes * NODE_HEIGHT + 200}>
            <Path
              d={svgPathData}
              stroke="#E2E8F0"
              strokeWidth="44"
              strokeLinecap="round"
              fill="none"
            />
            <Path
              d={svgPathData}
              stroke="white"
              strokeWidth="6"
              strokeDasharray="18 18"
              strokeLinecap="round"
              fill="none"
            />
          </Svg>
        </View>

        {sets.map((item, index) => (
          <View
            key={`set-${item.id}`}
            style={{
              height: NODE_HEIGHT,
              transform: [
                { translateX: index % 2 === 0 ? OFFSET_X : -OFFSET_X },
              ],
              alignItems: "center",
            }}
          >
            <NodeCard data={item} />
          </View>
        ))}

        <View
          style={{
            height: NODE_HEIGHT,
            transform: [
              { translateX: sets.length % 2 === 0 ? OFFSET_X : -OFFSET_X },
            ],
            alignItems: "center",
          }}
        >
          <ExamNode
            locked={sets.some(
              (s) => s.status === "active" || s.status === "locked",
            )}
          />
        </View>
      </ScrollView>

      <Footer />
    </View>
  );
}

function NodeCard({ data }: { data: any }) {
  const isActive = data.status === "active";
  const isLocked = data.status === "locked";
  const isGoodScore = data.score >= 74;

  return (
    <TouchableOpacity
      activeOpacity={0.8}
      className={`w-60 p-4 rounded-3xl bg-white border shadow-sm relative
        ${isActive ? "border-[#1544b2] border-2 scale-105" : "border-slate-100"}`}
    >
      <View className="flex-row items-center">
        <View
          className={`w-12 h-12 rounded-2xl items-center justify-center 
          ${isActive ? "bg-[#1544b2]" : "bg-slate-50"}`}
        >
          <MaterialIcons
            name="description"
            size={24}
            color={isActive ? "white" : isLocked ? "#cbd5e1" : "#1544b2"}
          />
        </View>

        <View className="ml-4 flex-1">
          <Text
            className={`font-bold text-sm ${isLocked ? "text-slate-400" : "text-slate-800"}`}
          >
            {data.title}
          </Text>
          {data.status === "completed" ? (
            <Text
              className={`text-xs font-black ${isGoodScore ? "text-[#10b981]" : "text-red-500"}`}
            >
              WYNIK: {data.score}%
            </Text>
          ) : (
            <Text className="text-[10px] text-slate-400 uppercase tracking-tighter">
              {isLocked ? "Zablokowane" : "Dostępne"}
            </Text>
          )}
        </View>

        {data.status === "completed" && (
          <MaterialIcons
            name="stars"
            size={24}
            color={isGoodScore ? "#f59e0b" : "#cbd5e1"}
          />
        )}
      </View>

      {isLocked && (
        <View className="absolute inset-0 bg-white/50 rounded-3xl items-center justify-center">
          <MaterialIcons name="lock" size={20} color="#94a3b8" />
        </View>
      )}
    </TouchableOpacity>
  );
}

function ExamNode({ locked }: { locked: boolean }) {
  return (
    <TouchableOpacity
      activeOpacity={0.8}
      disabled={locked}
      className={`w-64 p-5 rounded-3xl items-center justify-center shadow-lg
        ${locked ? "bg-slate-200" : "bg-amber-500"}`}
    >
      <MaterialIcons
        name="emoji-events"
        size={32}
        color={locked ? "#94a3b8" : "white"}
      />
      <Text
        className={`font-black mt-2 tracking-widest ${locked ? "text-slate-400" : "text-white"}`}
      >
        EGZAMIN KOŃCOWY
      </Text>
      {locked && (
        <Text className="text-[10px] text-slate-500 mt-1 uppercase">
          Ukończ wszystkie zestawy
        </Text>
      )}
    </TouchableOpacity>
  );
}
