from bs4 import BeautifulSoup
from selenium.webdriver.common.by import By
import time
import undetected_chromedriver as uc
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from urllib3.exceptions import NewConnectionError
from selenium.common.exceptions import WebDriverException
from selenium.webdriver.chrome.options import Options
import datetime
from dateutil.parser import parse
import pandas as pd
from selenium.common.exceptions import TimeoutException
import json
import requests
import os
import sys

class Lamborghini:

    def __init__(self, version_main=140):
        self.version_main= version_main
        # stop_date_str = pd.read_csv('./天下雜誌/天下雜誌_update.csv')['date'][0]
        # self.stopLineDate = pd.to_datetime(stop_date_str)  
        self.page = 1
        self.articles_list = []
        self.stop_scraping = False # 標記法

    # 拿來關閉阻擋的cookies,因為會使程式無法繼續
    def cookies_block(self,driver):

        try:
            cookies_button = WebDriverWait(driver,5).until(EC.presence_of_element_located((By.ID,'onetrust-accept-btn-handler')))
            cookies_button.click()
            print('✅成功移除cookies阻攔')
        except:
            pass


    def start_driver(self):
        """啟動瀏覽器"""
        print("--------------- 啟動瀏覽器 --------------")
        try:        
            # 能夠通過 Cloudflare
            options = Options()
            # options.add_argument("--incognito")  # 無痕模式
            driver = uc.Chrome(version_main=self.version_main,options=options)
            driver.get(f"https://www.balenciaga.com/en-us/men/discover/discover-balenciaga-%7C-automobili-lamborghini")
            driver.set_page_load_timeout(15) # 不出來最多等15秒
            wait = WebDriverWait(driver,5)

        except Exception as e:
            print(f'❌瀏覽出問題: {e}')

        

        print("--------------- 登入頁面 --------------")

        self.cookies_block(driver)


        time.sleep(2)
        driver.refresh()

        

        print("--------------- 選擇品項 --------------")

        self.cookies_block(driver)

        x = 500
        for i in range(8): #150

            driver.execute_script(f"window.scrollTo(0, {x});")
            x+= 500
            time.sleep(1.5)

        self.cookies_block(driver)

        container = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR,'li.l-productgrid__item')))
        print(len(container))
        data = []
        for i in range(0,2*len(container),2):
            driver.refresh()
            print('第: ',i)
            time.sleep(2)
            x = 400
            for _ in range(6): 
                driver.execute_script(f"window.scrollTo(0, {x});")
                x+= 500
                time.sleep(1)
            con = wait.until(EC.presence_of_all_elements_located((By.CSS_SELECTOR,'li.l-productgrid__item')))
            print('con:',len(con))
            con = con[i]
            price = con.text.split('\n')[-1]
            print(price)
            con.click()
            title = wait.until(EC.presence_of_element_located((By.CSS_SELECTOR,'h1.c-product__name')))
            title = title.text
            print(title)
            product_detail = driver.find_element(By.CSS_SELECTOR,'button.c-accordion__trigger')
            product_detail.click()
            detail = driver.find_element(By.ID,'accordionPanelDetails').text
            print(detail)
            print('---------------------')
            imgs = driver.find_element(By.CSS_SELECTOR,'ul.c-productcarousel__wrapper').find_elements(By.TAG_NAME,'img')
            x = 1000
            for _ in range(len(imgs)): 
                driver.execute_script(f"window.scrollTo(0, {x});")
                x+=900
                time.sleep(1)
            imgs = driver.find_element(By.CSS_SELECTOR,'ul.c-productcarousel__wrapper').find_elements(By.TAG_NAME,'img')
            print('len(imgs): ',len(imgs))
            img_list = []
            for img in imgs:
                img = img.get_attribute('src')
                img_list.append(img)
                print(img)
            
            data.append({'title':title,'detail':detail,'price':price,'img':img_list})
            
            with open('./Balenciaga_Lambo_100.json','w',encoding='utf-8') as f:
                json.dump(data,f,indent=4,ensure_ascii=False)
                
            driver.back()
      
        
                
            
                
        
    def download_img(self):

        # 設定 logger
        class Logger:
            def __init__(self, filename="output.log"):
                self.terminal = sys.stdout   # 原本的 console
                self.log = open(filename, "a", encoding="utf-8")

            def write(self, message):
                self.terminal.write(message)   # 照樣印到 console
                self.log.write(message)        # 同時寫進 log
                self.log.flush()               # 立即寫入檔案

            def flush(self):
                pass  # for Python buffer 需求
        
        # 把 stdout 和 stderr 都導向 Logger
        sys.stdout = Logger("./Urus_SE.log")
        sys.stderr = sys.stdout

        try:

            df = pd.read_json("./info_json/Balenciaga_Lambo_28.json")

            for idx,info in df.iterrows():
                
                try:
                    print(idx)
                    id = info['idx']
                    path_count = 1
                    path = f"./Balenciaga_Lambo_pic/{id}"
                    while os.path.exists(path):
                        path = f"./Balenciaga_Lambo_pic/{id}/{path_count}"
                        path_count += 1

                    os.makedirs(path, mode=0o777)
                    
                    imgs = info["img"]
                
                except Exception as e:
                    print(f'❌建立資料夾出問題: {e}')

                count = 0
                
                for img in imgs:
                    try:
                        
                        thumbnail = requests.get(img)
                        count += 1
                        
                        file: str = os.path.join(path,str(count))
                        with open(f'{file}.png', 'wb') as f:
                            f.write(thumbnail.content)

                    except Exception as e:
                        print(f'❌下載單張圖片出問題: {e}')

                print("✅成功抓到完整圖片")
                
                
                
            

            
        except Exception as e:
            print(f'❌下載圖片出問題: {e}')
    

if __name__ == "__main__":
    b = Lamborghini()
    b.download_img()

    # df = pd.read_json('./Balenciaga_Lambo.json')
    # df1 = pd.read_json('./Balenciaga_Lambo_1.json')
    # df2 = pd.read_json('./Balenciaga_Lambo_2.json')

    # final = pd.concat([df,df1,df2]).reset_index(drop=True)
    # final.to_json('./Balenciaga_Lambo_28.json',indent=4,force_ascii=False,orient="records")

    # with open('./info_json/Balenciaga_Lambo_28.json', 'r', encoding='utf-8') as f:
    #     text = json.load(f)
    
    # for id, t in enumerate(text):
    #     t['idx'] = id

    # with open('./info_json/Balenciaga_Lambo_28.json', 'w', encoding='utf-8') as f:
    #     json.dump(text, f, indent=4, ensure_ascii=False)

    # text = text.replace("\\/","/")

    # with open('./Balenciaga_Lambo_28.json', 'w', encoding='utf-8') as f:
    #     f.write(text)

    # df = pd.read_json('./Urus_SE.json')

    # count = 1
    # for idx, info in df.iterrows():
    #     if len(info["img"])==10:
    #         print(info['color'],info["color_subname"])
    #         print(count)
    #         count +=1



