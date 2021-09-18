import StatisticsDataSet from './statisticsDataSet';

export default class StatisticsData<T> {
  labels: string[] = [];
  datasets: StatisticsDataSet<T>[] = [];
}
