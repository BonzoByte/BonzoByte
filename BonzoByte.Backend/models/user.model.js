import mongoose from 'mongoose';
import bcrypt from 'bcryptjs';

const userSchema = new mongoose.Schema({
    // ✅ Osnovni podaci
    name: { type: String, required: true, trim: true },
    nickname: { type: String, trim: true, unique: true, sparse: true },
    email: { type: String, required: false, unique: true, lowercase: true, trim: true },

    // ✅ Autentikacija
    password: {
        type: String,
        required: function () { return !(this.googleId || this.facebookId); },
    },

    // ✅ Poveznice / identitet
    country: { type: Number, ref: 'Country' },
    googleId: { type: String, unique: true, sparse: true },
    facebookId: { type: String, unique: true, sparse: true },
    createdVia: { type: String, enum: ['manual', 'google', 'facebook'], default: 'manual' },
    provider: { type: [String], default: [] }, // npr. ['local','google']

    // ✅ Uloge i status
    isAdmin: { type: Boolean, default: false },
    isUser: { type: Boolean, default: false },
    isVerified: { type: Boolean, default: false },
    isOnline: { type: Boolean, default: false },

    // ✅ Dodatni info
    avatarUrl: { type: String, default: null },
    countryTPId: { type: Number, ref: 'Country' },
    gender: { type: String, enum: ['male', 'female', 'other'], default: 'other' },
    dateOfBirth: { type: Date },
    lastLogin: { type: Date },

    // ✅ Player povezivanje
    player1TPId: { type: Number, ref: 'Player' },
    player2TPId: { type: Number, ref: 'Player' },

    // ✅ Reset lozinke
    resetPasswordToken: { type: String },
    resetPasswordExpires: { type: Date },
    tokenVersion: { type: Number, default: 0 },
}, { timestamps: true });

// 🛡️ Hash lozinke (ako je promijenjena)
userSchema.pre('save', async function (next) {
    if (!this.isModified('password') || !this.password) return next();
    try {
        const salt = await bcrypt.genSalt(10);
        this.password = await bcrypt.hash(this.password, salt);
        next();
    } catch (err) {
        next(err);
    }
});

const User = mongoose.model('User', userSchema);
export default User;