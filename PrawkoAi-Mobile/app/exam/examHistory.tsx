import { MaterialCommunityIcons, MaterialIcons } from "@expo/vector-icons";
import { useRouter } from "expo-router";
import React, { useContext, useEffect, useState } from "react";
import {
  ActivityIndicator,
  FlatList,
  Platform,
  StatusBar,
  Text,
  TouchableOpacity,
  View,
} from "react-native";
import { useSafeAreaInsets } from "react-native-safe-area-context";
import { AuthContext } from "../_layout";
import Footer from "../components/footer";
import { useError } from "../context/errorContext";
import api from "../utils/api";

interface ExamHistory {
  userId: string;
  examSessionId: string;
  startedAt: string;
  finishedAt: string;
  score: number;
  isPassed: boolean;
  correctAnswersCount: number;
}

interface PagedResponse<T> {
  items: T[];
  pageNumber: number;
  totalCount: number;
  totalPages: number;
  hasNextPage: boolean;
}

export default function ExamHistoryScreen() {
  const insets = useSafeAreaInsets();
  const router = useRouter();
  const { user } = useContext(AuthContext);
  const { showError } = useError();

  const [exams, setExams] = useState<ExamHistory[]>([]);
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [hasNextPage, setHasNextPage] = useState(true);

  const PAGE_SIZE = 10;

  const paddingTop =
    Platform.OS === "android"
      ? insets.top > 0
        ? insets.top
        : StatusBar.currentHeight || 24
      : insets.top;

  const averageScore =
    exams.length > 0
      ? Math.round(
          exams.reduce((sum, exam) => sum + (exam.score || 0), 0) /
            exams.length,
        )
      : 0;

  const fetchExams = async (pageNum: number, isInitial = false) => {
    if (!user?.id) return;

    if (isInitial) setLoading(true);
    else setLoadingMore(true);

    try {
      const response = await api.get<PagedResponse<ExamHistory>>(
        "/api/exam/userHistory",
        {
          params: {
            userId: user.id,
            pageNumber: pageNum,
            pageSize: PAGE_SIZE,
          },
        },
      );

      const { items, totalCount: total, hasNextPage: next } = response.data;

      setExams((prev) => (isInitial ? items : [...prev, ...items]));
      setTotalCount(total);
      setHasNextPage(next);
      setPage(pageNum);
    } catch (error) {
      showError("Wystąpił błąd podczas pobierania historii egzaminów");
      console.error("Error fetching exam history:", error);
    } finally {
      setLoading(false);
      setLoadingMore(false);
    }
  };

  useEffect(() => {
    fetchExams(1, true);
  }, [user?.id]);

  const handleLoadMore = () => {
    if (!loadingMore && hasNextPage) {
      fetchExams(page + 1);
    }
  };

  const renderHeader = () => (
    <View>
      <View className="p-4 flex-row gap-4">
        <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-blue-900/10 shadow-sm">
          <Text className="text-sm text-slate-500 dark:text-slate-400 font-medium">
            Średni wynik
          </Text>
          <View className="flex-row items-baseline mt-1">
            <Text className="text-2xl font-bold text-[#1544b2]">
              {averageScore}
            </Text>
            <Text className="text-sm text-slate-400 ml-1">/74 pkt</Text>
          </View>
          <View className="mt-2 h-1 w-full bg-slate-100 dark:bg-slate-700 rounded-full overflow-hidden">
            <View
              className="bg-[#1544b2] h-full"
              style={{ width: `${(averageScore / 74) * 100}%` }}
            />
          </View>
        </View>

        <View className="flex-1 bg-white dark:bg-slate-800 p-4 rounded-xl border border-blue-900/10 shadow-sm">
          <Text className="text-sm text-slate-500 dark:text-slate-400 font-medium">
            Ukończone testy
          </Text>
          <Text className="text-2xl font-bold text-[#1544b2] mt-1">
            {totalCount}
          </Text>
        </View>
      </View>

      <View className="px-4 py-2">
        <Text className="text-lg font-bold text-slate-800 dark:text-slate-200">
          Twoje podejścia
        </Text>
      </View>
    </View>
  );

  const renderItem = ({ item }: { item: ExamHistory }) => (
    <View className="px-4">
      <TouchableOpacity
        className="bg-white dark:bg-slate-800 p-4 rounded-xl border border-blue-900/5 shadow-sm mb-3"
        onPress={() => router.push(`/exam/examResult/${item.examSessionId}`)}
      >
        <View className="flex-row justify-between items-start">
          <View className="flex-row items-center">
            <View
              className={`w-12 h-12 rounded-lg items-center justify-center ${
                item.isPassed
                  ? "bg-green-50 dark:bg-green-900/20"
                  : "bg-red-50 dark:bg-red-900/20"
              }`}
            >
              <MaterialIcons
                name={
                  item.isPassed ? "assignment-turned-in" : "assignment-late"
                }
                size={24}
                color={item.isPassed ? "#16a34a" : "#dc2626"}
              />
            </View>
            <View className="ml-3">
              <Text className="font-bold text-slate-900 dark:text-white">
                {new Date(item.finishedAt).toLocaleString("pl-PL", {
                  day: "2-digit",
                  month: "long",
                  year: "numeric",
                  hour: "2-digit",
                  minute: "2-digit",
                })}
              </Text>
              <Text className="text-sm text-slate-500">
                Wynik:{" "}
                <Text className="font-semibold text-slate-700 dark:text-slate-300">
                  {item.score} pkt
                </Text>
              </Text>
            </View>
          </View>
          <View
            className={`${
              item.isPassed
                ? "bg-green-100 dark:bg-green-900/30"
                : "bg-red-100 dark:bg-red-900/30"
            } px-3 py-1 rounded-full`}
          >
            <Text
              className={`text-[10px] font-bold uppercase ${
                item.isPassed
                  ? "text-green-700 dark:text-green-400"
                  : "text-red-700 dark:text-red-400"
              }`}
            >
              {item.isPassed ? "POZYTYWNY" : "NEGATYWNY"}
            </Text>
          </View>
        </View>
      </TouchableOpacity>
    </View>
  );

  const renderFooter = () => {
    if (!loadingMore) return <View style={{ height: 100 }} />;
    return (
      <View className="py-6">
        <ActivityIndicator color="#1544b2" />
      </View>
    );
  };

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
            className="p-2"
            onPress={() =>
              router.canGoBack() ? router.back() : router.replace("/dashboard")
            }
          >
            <MaterialCommunityIcons
              name="arrow-left"
              size={24}
              color="#1544b2"
            />
          </TouchableOpacity>
          <Text className="text-xl font-bold ml-2 flex-1 text-slate-900 dark:text-white">
            Historia Egzaminów
          </Text>
          <TouchableOpacity className="p-2">
            <MaterialCommunityIcons
              name="filter-variant"
              size={24}
              color="#1544b2"
            />
          </TouchableOpacity>
        </View>
      </View>

      {loading ? (
        <View className="flex-1 items-center justify-center">
          <ActivityIndicator size="large" color="#1544b2" />
        </View>
      ) : (
        <FlatList
          data={exams}
          renderItem={renderItem}
          keyExtractor={(item) => item.examSessionId}
          ListHeaderComponent={renderHeader}
          ListFooterComponent={renderFooter}
          onEndReached={handleLoadMore}
          onEndReachedThreshold={0.5}
          showsVerticalScrollIndicator={false}
          contentContainerStyle={{
            paddingBottom: Platform.OS === "ios" ? 40 : 20,
          }}
        />
      )}

      <Footer />
    </View>
  );
}
