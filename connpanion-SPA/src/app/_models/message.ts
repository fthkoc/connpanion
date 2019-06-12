export interface Message {
    id: number;
    senderID: number;
    senderKnownAs: string;
    senderPhotoURL: string;
    receiverID: number;
    receiverKnownAs: string;
    receiverPhotoURL: string;
    content: string;
    isRead: boolean;
    dateRead: Date;
    messageSent: Date;
}
