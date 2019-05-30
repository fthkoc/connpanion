import { Photo } from './photo';

export interface User {
    id: number;
    userName: string;
    knownAs: string;
    age: number;
    gender: string;
    dateCreated: Date;
    lastActive: Date;
    photoURL: string;
    city: string;
    country: string;
    interests?: string;
    introduction?: string;
    lookingFor?: string;
    photos?: Photo[];
}
