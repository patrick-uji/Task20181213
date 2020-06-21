//https://stackoverflow.com/a/45188213
export interface Dictionary<T> {
  [Key: string]: T;
}
export class Utils {
  static addDaysToDate(date: Date, days: number): Date {
    date.setDate(date.getDate() + days);
    return date;
  }
  static getISODateOnly(date: Date): string {
    return date.toISOString().substring(0, 10);
  }
}
