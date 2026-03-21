export interface PaginatedList<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
}

export interface DeckDto {
  id: string;
  title: string;
  description: string | null;
}

export interface VocabEntryDto {
  id: string;
  term: string;
  definition: string | null;
  englishTranslation: string | null;
  imageUrl: string | null;
  example: string | null;
  isClassified: boolean;
  type: string;
}

export interface NounDto extends VocabEntryDto {
  gender: number;
  pluralForm: string | null;
  isSingularOnly: boolean;
  isPluralOnly: boolean;
}

export interface CourseDto {
  id: string;
  title: string;
  description: string | null;
}

export const GenderLabels: Record<number, string> = {
  0: 'der',
  1: 'die',
  2: 'das',
  3: 'der/die',
};
