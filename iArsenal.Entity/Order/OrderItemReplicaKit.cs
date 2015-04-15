﻿using System;

namespace iArsenal.Entity
{
    public class OrdrItmReplicaKit : OrderItem
    {
        public OrdrItmReplicaKit() { }

        public OrdrItmReplicaKit(int id) : base(id) { }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
        }

        public override void Place(Member m, Product p, System.Data.SqlClient.SqlTransaction trans = null)
        {
            base.Place(m, p, trans);
        }
    }

    public class OrdrItmReplicaKitHome : OrdrItmReplicaKit
    {
        public OrdrItmReplicaKitHome() { }

        public OrdrItmReplicaKitHome(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.ReplicaKitHome))
                throw new Exception("The OrderItem is not the type of ReplicaKitHome.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.ReplicaKitHome));

            base.Place(m, product, trans);
        }
    }

    public class OrdrItemReplicaKitAway : OrdrItmReplicaKit
    {
        public OrdrItemReplicaKitAway() { }

        public OrdrItemReplicaKitAway(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.ReplicaKitAway))
                throw new Exception("The OrderItem is not the type of ReplicaKitAway.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.ReplicaKitAway));

            base.Place(m, product, trans);
        }
    }

    public class OrdrItmReplicaKitCup : OrdrItmReplicaKit
    {
        public OrdrItmReplicaKitCup() { }

        public OrdrItmReplicaKitCup(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.ReplicaKitCup))
                throw new Exception("The OrderItem is not the type of ReplicaKitCup.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.ReplicaKitCup));

            base.Place(m, product, trans);
        }
    }

    public class OrdrItmPlayerNumber : OrderItem
    {
        public OrdrItmPlayerNumber() { }

        public OrdrItmPlayerNumber(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.PlayerNumber))
                throw new Exception("The OrderItem is not the type of PlayerNumber.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.PlayerNumber));

            base.Place(m, product, trans);
        }

        #region Members and Properties

        public string PrintingNumber
        {
            get { return Size; }
            set { Size = value; }
        }

        public Guid ArsenalPlayerGuid
        {
            get
            {
                if (!string.IsNullOrEmpty(Remark))
                {
                    try { return new Guid(Remark); }
                    catch { throw new Exception("Can't get the Partner of OrderItem_PlayerNumber.Remark"); }
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        #endregion
    }

    public class OrdrItmPlayerName : OrderItem
    {
        public OrdrItmPlayerName() { }

        public OrdrItmPlayerName(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.PlayerName))
                throw new Exception("The OrderItem is not the type of PlayerName.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.PlayerName));

            base.Place(m, product, trans);
        }

        #region Members and Properties

        public string PrintingName
        {
            get { return Size; }
            set { Size = value; }
        }

        public Guid ArsenalPlayerGuid
        {
            get
            {
                if (!string.IsNullOrEmpty(Remark))
                {
                    try { return new Guid(Remark); }
                    catch { throw new Exception("Can't get the Partner of OrderItem_PlayerName.Remark"); }
                }
                else
                {
                    return Guid.Empty;
                }
            }
        }

        #endregion
    }

    public class OrdrItmArsenalFont : OrderItem
    {
        public OrdrItmArsenalFont() { }

        public OrdrItmArsenalFont(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.ArsenalFont))
                throw new Exception("The OrderItem is not the type of ArsenalFont.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.ArsenalFont));

            base.Place(m, product, trans);
        }
    }

    public class OrdrItmPremiershipPatch : OrderItem
    {
        public OrdrItmPremiershipPatch() { }

        public OrdrItmPremiershipPatch(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.PremiershipPatch))
                throw new Exception("The OrderItem is not the type of PremiershipPatch.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.PremiershipPatch));

            base.Place(m, product, trans);
        }
    }

    public class OrdrItmChampionshipPatch : OrderItem
    {
        public OrdrItmChampionshipPatch() { }

        public OrdrItmChampionshipPatch(int id) : base(id) { this.Init(); }

        private void Init()
        {
            if (ProductGuid == null)
                throw new Exception("Loading OrderItem failed.");

            Product p = Product.Cache.Load(ProductGuid);

            if (!p.ProductType.Equals(ProductType.ChampionshipPatch))
                throw new Exception("The OrderItem is not the type of ChampionshipPatch.");
        }

        public override void Mapper(object obj)
        {
            base.Mapper(obj);
            this.Init();
        }

        public void Place(Member m, System.Data.SqlClient.SqlTransaction trans = null)
        {
            Product product = Product.Cache.ProductList.Find(p =>
                p.ProductType.Equals(ProductType.ChampionshipPatch));

            base.Place(m, product, trans);
        }
    }
}
